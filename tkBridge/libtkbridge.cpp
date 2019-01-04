#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <errno.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <net/if.h>
#include <linux/can.h>
#include <linux/can/raw.h>
#include "dbc/tesla_can.h"


extern "C" {

struct CANframe_t {
   struct can_frame frame;  // socketCAN frame
   uint64_t stamp;          // timestamp of message when read

   // return integer ID
   int     id()     { return frame.can_id; }
   
   // return data as a uint64_t pointer, useful for DBC encoding
   uint64_t *data() { return reinterpret_cast<uint64_t*>(frame.data); }

    void reset() { *data() = 0; }
};


int can_soc = -1;

/**
    Init can
*/
bool tkbridge_can_init(char *port) {

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

bool tkbridge_can_close() {
    int err = close(can_soc);
    if(err == -1) {
        //printf("CAN close ERROR: %s\n", strerror(errno));
        return false;
    } else {
        return true;
    }
}

bool tkbridge_can_write_fd(struct can_frame frame) {

    int retval;
    retval = write(can_soc, &frame, sizeof(struct can_frame));

    if(retval == -1) {
        printf("CAN write ERROR: %s\n", strerror(errno));
    }

    return retval == sizeof(struct can_frame);
}

bool tkbridge_can_write(int id, int dlc, uint64_t data) {

    struct can_frame frame;
    frame.can_id = id;
    frame.can_dlc = dlc;
    *reinterpret_cast<uint64_t*>(frame.data) = data;

    return tkbridge_can_write_fd(frame);
}


bool tkbridge_can_read(struct can_frame *frame) {
    int retval = 0;

    retval = read(can_soc, frame, sizeof(struct can_frame));
    bool ok = retval == sizeof(struct can_frame);
    //if(ok) {
    //    struct timeval tv;
    //    ioctl(soc, SIOCGSTAMP, &tv);
    //    msg->stamp = tv2TimeStamp(tv);
    //}
    return ok;
}



/**
 *  write values in order:
 *  - steer angle 
 *  - speed
 *  
 */
bool tkbridge_can_write_vals(float *vals, int n_vals) {

    if(n_vals < 2)
        return false;

    CANframe_t msg;

    can_0x003_STW_ANGL_STAT_t ANGL_STAT;
    can_0x155_ESP_B_t ESP_B;

    // steer angle
    msg.frame.can_id = 0x003;
    msg.frame.can_dlc = 8;    
    encode_can_0x003_StW_Angl(&ANGL_STAT, vals[0]);
    pack_can_0x003_STW_ANGL_STAT(&ANGL_STAT, msg.data()); // fill data 
    tkbridge_can_write_fd(msg.frame); 

    // speed
    msg.frame.can_id = 0x155;
    msg.frame.can_dlc = 8;    
    encode_can_0x155_ESP_vehicleSpeed(&ESP_B, vals[1]);
    pack_can_0x155_ESP_B(&ESP_B, msg.data()); // fill data 
    tkbridge_can_write_fd(msg.frame);

    return true;
}

}
