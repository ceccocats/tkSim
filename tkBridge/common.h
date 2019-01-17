#include <stdio.h>
#include <stdint.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>


struct CANframe_t {
    int id; 
    int dlc;
    uint64_t data;
    uint64_t stamp; // timestamp of message when read
};
 
bool can_private_write_vals(float *vals);
bool can_private_read_vals(float *vals);

extern "C" {
    bool tkbridge_can_write_fd(struct CANframe_t frame);
    bool tkbridge_can_read(struct CANframe_t *frame);
}


