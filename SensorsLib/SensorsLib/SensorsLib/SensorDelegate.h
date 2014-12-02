#pragma once
#include "OpenSpatialServiceController.h"
#include <map>
#include <mutex> 

class SensorDelegate : public OpenSpatialDelegate
{

public:
	std::map<int, Pose6DEvent*> pose6DEventsMap;

public:
	SensorDelegate();
	~SensorDelegate();
	virtual void pointerEventFired(PointerEvent event);
	virtual void gestureEventFired(GestureEvent event);
	virtual void pose6DEventFired(Pose6DEvent event);
	virtual void buttonEventFired(ButtonEvent event);

private:
	std::mutex mMtx;
};

