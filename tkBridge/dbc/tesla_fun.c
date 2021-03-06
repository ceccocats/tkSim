#include "../common.h"
#include "tesla_can.h"

bool can_private_write_vals(float *vals) {

    bool status = true;

    CANframe_t msg;
    can_0x003_STW_ANGL_STAT_t ANGL_STAT;
    can_0x155_ESP_B_t ESP_B;

    // steer angle
    msg.id = 0x003;
    msg.dlc = 8;    
    encode_can_0x003_StW_Angl(&ANGL_STAT, vals[0]);
    pack_can_0x003_STW_ANGL_STAT(&ANGL_STAT, &msg.data); // fill data 
    status = status && tkbridge_can_write_fd(msg); 

    // speed
    msg.id = 0x155;
    msg.dlc = 8;    
    encode_can_0x155_ESP_vehicleSpeed(&ESP_B, vals[1]*3.6);
    pack_can_0x155_ESP_B(&ESP_B, &msg.data); // fill data 
    status = status && tkbridge_can_write_fd(msg);
    
    return status;
}

bool can_private_read_vals(float *vals) {

    bool status = true;

    vals[0] = 0;
    vals[1] = 0;
    return status;
}