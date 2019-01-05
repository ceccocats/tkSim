all: libtkbridge

UNITY_DIR=/home/cecco/Programmi/Unity2018/Editor/
CAN_LIB=dbc/libteslacan.so
#CAN_LIB=dbc_hipertcar/libhipertcarcan.so


CAN_LIB_PATH=`dirname $(CAN_LIB)`

libtkbridge: libtkbridge.cpp 
	$(MAKE) -C $(CAN_LIB_PATH)
	g++ libtkbridge.cpp $(CAN_LIB) `pkg-config --libs opencv` -shared -fPIC -o libtkbridge.so

test: libtkbridge
	g++ test.cpp libtkbridge.so $(CAN_LIB) -o test

clean:
	rm -f *.so *~
	$(MAKE) -C $(CAN_LIB_PATH) clean

install: libtkbridge	
	cp  *.so $(CAN_LIB) $(UNITY_DIR)