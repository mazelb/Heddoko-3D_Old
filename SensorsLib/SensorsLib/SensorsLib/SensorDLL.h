#pragma once

#ifdef SENSORSDLL_EXPORTS
#define SENSORSSDLL_API __declspec(dllexport) 
#else
#define SENSORSSDLL_API __declspec(dllimport) 
#endif

#include "SensorDelegate.h"
#include "OpenSpatialServiceController.h"

using namespace std;

class SENSORSSDLL_API SensorDLL
{
public:
	SensorDLL();
	~SensorDLL();

	int  getNumberConnectedDevices();
	void initSensorsConnection();
	void shutDown6DSensor(unsigned int vIdx);
	void connect6DSensor(unsigned int vIdx);

	Pose6DEvent* getSensorLatestEvent(unsigned int vIdx);

private:
	OpenSpatialServiceController* mpController;
	SensorDelegate* mpDelegate;

private: 
	BOOL indexExist(unsigned int vIdx);
};

//Recommended skeleton connections: 
// Idx		Description