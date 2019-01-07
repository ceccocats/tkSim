#include <fcntl.h>
#include <sys/ioctl.h>
#include <net/if.h>
#include <linux/can.h>
#include <linux/can/raw.h>

extern "C" {

// vars
static int can_soc = -1;

bool native_can_init(char *port) {
    // init virtual device
    if(system("ifconfig | grep vcan0") != 0)
        system("gksu -- bash -c 'modprobe vcan; ip link add dev vcan0 type vcan; ip link set down vcan0; ip link set up vcan0'");

    struct ifreq ifr;
    struct sockaddr_can addr;

    /* open socket */
    can_soc = socket(PF_CAN, SOCK_RAW, CAN_RAW);
    if(can_soc < 0) {
        return false;
    }

    addr.can_family = AF_CAN;
    strcpy(ifr.ifr_name, port);

    if (ioctl(can_soc, SIOCGIFINDEX, &ifr) < 0) {
        return false;
    }

    addr.can_ifindex = ifr.ifr_ifindex;

    if(fcntl(can_soc, F_SETFL, O_NONBLOCK) < 0) {
        return false;
    }

    if (bind(can_soc, (struct sockaddr *)&addr, sizeof(addr)) < 0) {
        return false;
    }
    return true;
}

bool native_can_close() {

    int err = close(can_soc);
    if(err == -1) {
        //printf("CAN close ERROR: %s\n", strerror(errno));
        return false;
    } else {
        return true;
    }
}

bool native_can_write(struct CANframe_t msg) {

    struct can_frame frame;
    frame.can_id = msg.id;
    frame.can_dlc = msg.dlc;
    *reinterpret_cast<uint64_t*>(frame.data) = msg.data; 

    if(can_soc<0)
        return false;

    int retval;
    retval = write(can_soc, &frame, sizeof(struct can_frame));

    if(retval == -1) {
        printf("CAN write ERROR: %s\n", strerror(errno));
    }

    return retval == sizeof(struct can_frame);
}

bool native_can_read(struct CANframe_t *msg) {

    struct can_frame frame;
    
    if(can_soc<0)
        return false;

    int retval = 0;

    retval = read(can_soc, &frame, sizeof(struct can_frame));
    bool ok = retval == sizeof(struct can_frame);

    msg->id = frame.can_id;
    msg->dlc = frame.can_dlc;
    msg->data = *reinterpret_cast<uint64_t*>(frame.data);

    //if(ok) {
    //    struct timeval tv;
    //    ioctl(soc, SIOCGSTAMP, &tv);
    //    msg->stamp = tv2TimeStamp(tv);
    //}
    return ok;
}

}