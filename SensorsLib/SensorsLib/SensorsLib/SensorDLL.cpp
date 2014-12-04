#include "stdafx.h"
#include "SensorDLL.h"
#include <map>

SensorDLL::SensorDLL() :
	mpController(NULL),
	mpDelegate(NULL)
{
}

SensorDLL::~SensorDLL()
{
	for (int i = 0; i < getNumberConnectedDevices(); ++i)
	{
		shutDown6DSensor(i);
	}

	delete mpDelegate;
	delete mpController;
}

void SensorDLL::initSensorsConnection()
{
	if (mpController == NULL)
	{
		mpController = new OpenSpatialServiceController();
	}

	if (mpDelegate == NULL)
	{
		mpDelegate = new SensorDelegate();
	}
	
	mpController->setDelegate(mpDelegate);
}

void SensorDLL::shutDown6DSensor(unsigned int vIdx)
{
	if (indexExist(vIdx))
	{
		mpController->shutdown(mpController->names.at(vIdx));
	}
}

void SensorDLL::connect6DSensor(unsigned int vIdx)
{
	if (indexExist(vIdx))
	{
		Pose6DEvent* vTempEvent = new Pose6DEvent();
		mMtx.lock();
			mpDelegate->pose6DEventsMap.insert(std::pair<int, Pose6DEvent*>(vIdx, vTempEvent));
		mMtx.unlock();

		mpController->setMode(mpController->names.at(vIdx), MODE_3D);
		mpController->subscribeToPose6D(mpController->names.at(vIdx));
	}
}

Pose6DEvent* SensorDLL::getSensorLatestEvent(unsigned int vIdx)
{
	if (mpDelegate->pose6DEventsMap[vIdx] != NULL)
	{
		return mpDelegate->pose6DEventsMap[vIdx];
	}

	return NULL;
}


void SensorDLL::getSensorLatestOrientation(unsigned int vIdx, float &vPitch, float &vRoll, float &vYaw)
{
	if (mpDelegate->pose6DEventsMap[vIdx] != NULL)
	{
		vPitch = mpDelegate->pose6DEventsMap[vIdx]->pitch;
		vRoll = mpDelegate->pose6DEventsMap[vIdx]->roll;
		vYaw = mpDelegate->pose6DEventsMap[vIdx]->yaw;
	}
}


bool SensorDLL::indexExist(unsigned int vIdx)
{	
	if (mpController != NULL)
	{
		if (vIdx >= 0 && vIdx < mpController->names.size())
		{
			return true;
		}
	}

	return false;
}

int SensorDLL::getNumberConnectedDevices()
{
	if (mpController != NULL)
	{
		return mpController->getNames().size();
	}

	return 0;
}

extern "C"
{
	SensorDLL* sensorsLib = NULL;

	void initSensorsConnection()
	{
		//init sensors lib 
		sensorsLib = new SensorDLL();	
		sensorsLib->initSensorsConnection();
	}

	bool indexExist(int vIdx)
	{
		return sensorsLib->indexExist(vIdx);
	}
	
	void connect6DSensor(int vIdx)
	{
		if (sensorsLib->getNumberConnectedDevices() >= (vIdx + 1))
		{
			sensorsLib->connect6DSensor(vIdx);
		}
	}

	int  getNumberConnectedDevices()
	{
		return sensorsLib->getNumberConnectedDevices();
	}

	void shutDown6DSensor(int vIdx)
	{
		if (sensorsLib->getNumberConnectedDevices() >= (vIdx + 1))
		{
			sensorsLib->shutDown6DSensor(vIdx);
		}
	}	
	

	float getSensorLatestPitch(int vIdx)
	{
		if (sensorsLib->getNumberConnectedDevices() >= (vIdx + 1))
		{
			return sensorsLib->getSensorLatestEvent(vIdx)->pitch;
		}
	}

	float getSensorLatestRoll(int vIdx)
	{
		if (sensorsLib->getNumberConnectedDevices() >= (vIdx + 1))
		{
			return sensorsLib->getSensorLatestEvent(vIdx)->roll;
		}

	}

	float getSensorLatestYaw(int vIdx)
	{
		if (sensorsLib->getNumberConnectedDevices() >= (vIdx + 1))
		{
			return sensorsLib->getSensorLatestEvent(vIdx)->yaw;
		}

	}
}



