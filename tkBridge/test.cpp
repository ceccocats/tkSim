#include <iostream>
#include "common.h"

extern "C" {
    bool tkbridge_camera_init();
    bool tkbridge_can_init(char *port);
    bool tkbridge_can_write_vals(float *vals, int n_vals);
}

int main() {

    const char *port = "vcan0";
    float vals[2];
    bool status;
    
    status = tkbridge_can_init( (char*) port);
    printf("Can init: %d\n", status);
    
    status = tkbridge_can_write_vals(vals,2);
    printf("Can write: %d\n", status);

    status = tkbridge_camera_init();
    printf("Camera init: %d\n", status);

    return 0;
}