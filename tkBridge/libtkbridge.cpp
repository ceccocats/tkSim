#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <errno.h>

#include <iostream>
#include <fstream>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>

#include "common.h"

#ifdef WINDOWS
    // TODO
#else
    #include "socketcan.h"
#endif

extern "C" {

static std::fstream camera_fifo;

/**
    Init can
*/
bool tkbridge_can_init(char *port) {

    return native_can_init(port);
}

bool tkbridge_can_close() {

    return native_can_close();
}

bool tkbridge_can_write_fd(struct CANframe_t frame) {

    return native_can_write(frame);
}

bool tkbridge_can_write(int id, int dlc, uint64_t data) {

    struct CANframe_t frame;
    frame.id = id;
    frame.dlc = dlc;
    frame.data = data;

    return tkbridge_can_write_fd(frame);
}


bool tkbridge_can_read(struct CANframe_t *frame) {

    return native_can_read(frame);
}


/**
 *  write values in order:
 *  - steer angle 
 *  - speed
 *  
 */
bool tkbridge_can_write_vals(float *vals, int n_vals) {

    if(can_soc < 0 || n_vals < 5)
        return false;

    return can_private_write_vals(vals);
}

/** 
 *  read values in order:
 *  - steer angle
 *  - speed
 */
bool tkbridge_can_read_vals(float *vals, int n_vals) {

    if(can_soc < 0 || n_vals < 2)
        return false;

    return can_private_read_vals(vals);
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
