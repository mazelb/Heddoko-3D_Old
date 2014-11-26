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
		mpController->subscribeToPose6D(mpController->names.at(vIdx));
		mpController->setMode(mpController->names.at(vIdx), MODE_3D);
		Pose6DEvent* vTempEvent = new Pose6DEvent();
		mpDelegate->pose6DEventsMap.insert(std::pair<int, Pose6DEvent*>(vIdx, vTempEvent));
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

BOOL SensorDLL::indexExist(unsigned int vIdx)
{
	BOOL vResult = FALSE;
	
	if (mpController != NULL)
	{
		if (vIdx >= 0 && vIdx < mpController->names.size())
		{
			vResult = TRUE;
		}
	}

	return vResult;
}

int SensorDLL::getNumberConnectedDevices()
{
	if (mpController != NULL)
	{
		return mpController->getNames().size();
	}

	return 0;
}



