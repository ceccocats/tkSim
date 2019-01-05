all: libteslacan

libteslacan: tesla_can.c tesla_fun.c
	g++ tesla_fun.c tesla_can.c -shared -fPIC -o libteslacan.so

clean:
	rm -f *.so *~