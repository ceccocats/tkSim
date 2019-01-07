all: libtkbridge

# confs
UNITY_DIR=/home/cecco/Programmi/Unity2018/Editor/
CAN_LIB_PATH=dbc/
#CAN_LIB_PATH=dbc_hipertcar/

CAN_LIB_NAME=libcan.so
libtkbridge: libtkbridge.cpp 
	$(MAKE) -C $(CAN_LIB_PATH)
	g++ libtkbridge.cpp $(CAN_LIB_NAME) `pkg-config --libs opencv` -shared -fPIC -o libtkbridge.so

test: libtkbridge
	g++ test.cpp libtkbridge.so $(CAN_LIB_NAME) -o test

clean:
	rm -f *.so *~
	$(MAKE) -C $(CAN_LIB_PATH) clean

install: libtkbridge	
	cp  *.so $(UNITY_DIR)

uninstall:	
	rm  -f $(UNITY_DIR)/libtkbridge.so $(UNITY_DIR)/$(CAN_LIB_NAME)
