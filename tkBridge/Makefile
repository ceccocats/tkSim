all: libtkbridge

UNITY_DIR=/home/cecco/Programmi/Unity2018/Editor/

libtkbridge: libtkbridge.cpp
	g++ libtkbridge.cpp dbc/tesla_can.c -shared -fPIC -o libtkbridge.so

clean:
	rm libtkbridge.so *~

install: libtkbridge	
	cp  libtkbridge.so $(UNITY_DIR)
	

