#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <errno.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <net/if.h>
#include <linux/can.h>
#include <linux/can/raw.h>

#include <iostream>
#include <fstream>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>

#include "common.h"

extern "C" {

// vars
static int can_soc = -1;
static std::fstream camera_fifo;

/**
    Init can
*/
bool tkbridge_can_init(char *port) {

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

    if(can_soc<0)
        return false;

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

    if(can_soc<0)
        return false;

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

    if(can_soc < 0 || n_vals < 2)
        return false;

    return can_private_write_vals(vals);
}

bool tkbridge_camera_init() {

    // make fifo file for ffmpeg
    system("mkfifo /tmp/tkcamera0");

    const char *out = "/tmp/tkcamera0";
    camera_fifo.close();
    camera_fifo.open(out);

    if(!camera_fifo)
        return false;

    return true;
}

bool tkbridge_camera(uint8_t data[], int len) {
     
    if(!camera_fifo) {
        return false;
    }

    for (int i = 0; i < len; ++i)
        camera_fifo << data[i];

    return true;
}



}
