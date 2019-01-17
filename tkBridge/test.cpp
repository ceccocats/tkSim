#include <iostream>
#include "common.h"

extern "C" {
    bool tkbridge_camera_init();
    bool tkbridge_can_init(char *port);
    bool tkbridge_can_write_vals(float *vals, int n_vals);
    bool tkbridge_can_read_vals(float *vals, int n_vals);
}

int main() {

    const char *port = "can0";
    float vals[5];
    bool status;
    
    status = tkbridge_can_init( (char*) port);
    printf("Can init: %d\n", status);
    
    status = tkbridge_can_write_vals(vals,5);
    printf("Can write: %d\n", status);

    status = tkbridge_camera_init();
    printf("Camera init: %d\n", status);

    while(true) {    
        status = tkbridge_can_read_vals(vals,2);
        printf("Can read: %d\n", status);
        usleep(100000);
    }

    return 0;
}