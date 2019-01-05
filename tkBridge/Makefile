all: libtkbridge

UNITY_DIR=/home/cecco/Programmi/Unity2018/Editor/

libteslacan: dbc/tesla_can.c dbc/tesla_fun.c
	g++ dbc/tesla_fun.c dbc/tesla_can.c -shared -fPIC -o libteslacan.so

libtkbridge: libtkbridge.cpp libteslacan
	g++ libtkbridge.cpp -L. -lteslacan `pkg-config --libs opencv` -shared -fPIC -o libtkbridge.so

test: libtkbridge
	g++ test.cpp libtkbridge.so libteslacan.so -o test

clean:
	rm *.so *~

install: libtkbridge	
	cp  *.so $(UNITY_DIR)
	

