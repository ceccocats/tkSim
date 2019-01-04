// compile
// g++ -std=c++11 test.cpp `pkg-config --libs opencv` -o test
// redirect video to virtual device
// ./test lol.mp4 | ffmpeg -re -i pipe:0 -vcodec rawvideo -pix_fmt yuyv422 -threads 0 -f v4l2 /dev/video1

// with fifo
// mkfifo /tmp/tkcamera0
// ./test lol.mp4
// tail -c +1 -F /tmp/tkcamera0 | ffmpeg -i pipe:0 -vcodec rawvideo -pix_fmt yuyv422 -threads 0 -f v4l2 /dev/video1
#include <iostream>
#include <fstream>
#include <unistd.h>
#include <sys/types.h>
#include <sys/stat.h>

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>

using namespace cv;


int main(int argc, char *argv[]) {

    const char *out = "/tmp/tkcamera0";

    mkfifo(out, 0666);
    std::ofstream of(out);

    VideoCapture cap(argv[1]); // open the default camera
    if(!cap.isOpened())  // check if we succeeded
        return -1;

    int j=0;
    while(of && cap.isOpened()) {
        std::cout<<"frame "<<j++<<"\n";
        Mat frame;
        cap >> frame;
        std::vector<uchar> buff;
        vector<uchar> :: iterator i;    
        cv::imencode(".jpg", frame, buff);
        for (i = buff.begin(); i != buff.end(); ++i)
            of << *i ;
        usleep(66000);
    }
    of.close();

    return 0;
}