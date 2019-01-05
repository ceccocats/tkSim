#include <stdio.h>
#include <stdint.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <linux/can.h>
#include <linux/can/raw.h>

struct CANframe_t {
   struct can_frame frame;  // socketCAN frame
   uint64_t stamp;          // timestamp of message when read

   // return integer ID
   int     id()     { return frame.can_id; }
   
   // return data as a uint64_t pointer, useful for DBC encoding
   uint64_t *data() { return reinterpret_cast<uint64_t*>(frame.data); }

    void reset() { *data() = 0; }
};

bool can_private_write_vals(float *vals);

extern "C" {
    bool tkbridge_can_write_fd(struct can_frame frame);
}