#include "../common.h"
#include "tesla_can.h"

bool can_private_write_vals(float *vals) {

    bool status = true;

    CANframe_t msg;
    can_0x003_STW_ANGL_STAT_t ANGL_STAT;
    can_0x155_ESP_B_t ESP_B;

    // steer angle
    msg.frame.can_id = 0x003;
    msg.frame.can_dlc = 8;    
    encode_can_0x003_StW_Angl(&ANGL_STAT, vals[0]);
    pack_can_0x003_STW_ANGL_STAT(&ANGL_STAT, msg.data()); // fill data 
    status = status && tkbridge_can_write_fd(msg.frame); 

    // speed
    msg.frame.can_id = 0x155;
    msg.frame.can_dlc = 8;    
    encode_can_0x155_ESP_vehicleSpeed(&ESP_B, vals[1]);
    pack_can_0x155_ESP_B(&ESP_B, msg.data()); // fill data 
    status = status && tkbridge_can_write_fd(msg.frame);

    return status;
}