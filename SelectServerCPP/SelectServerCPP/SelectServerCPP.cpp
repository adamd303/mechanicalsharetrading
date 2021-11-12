// SelectServerCPP.cpp : Defines the entry point for the console application.
//

#include <WinSock2.h>


void main(void)
{
	WSADATA wsaData;

	// Initialise Winsock version 2.2
	int Ret;

	if ((Ret = WSAStartup(MAKEWORD(2.2), &wsaData)) != 0)
	{

	}
}

