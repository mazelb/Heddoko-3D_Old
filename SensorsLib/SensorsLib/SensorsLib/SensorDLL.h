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
	void getSensorLatestOrientation(unsigned int vIdx, float& vPitch, float& vRoll, float& vYaw);
	BOOL indexExist(unsigned int vIdx);

	Pose6DEvent* getSensorLatestEvent(unsigned int vIdx);

private:
	OpenSpatialServiceController* mpController;
	SensorDelegate* mpDelegate;
	std::mutex mMtx;
};

extern "C"
{
	SENSORSSDLL_API void initSensorsConnection();
	SENSORSSDLL_API BOOL indexExist(int vIdx);
	SENSORSSDLL_API void connect6DSensor(int vIdx);
	SENSORSSDLL_API void shutDown6DSensor(int vIdx);
	SENSORSSDLL_API int  getNumberConnectedDevices();
	//SENSORSSDLL_API void getSensorLatestOrientation(unsigned int vIdx, float& vPitch, float& vRoll, float& vYaw);
	
	SENSORSSDLL_API float getSensorLatestPitch(int vIdx);
	SENSORSSDLL_API float getSensorLatestRoll(int vIdx);
	SENSORSSDLL_API float getSensorLatestYaw(int vIdx);


}

//Recommended skeleton connections: 
// Idx		Description