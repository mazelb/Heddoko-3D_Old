#include "stdafx.h"
#include "SensorDelegate.h"


SensorDelegate::SensorDelegate()	
{
}


SensorDelegate::~SensorDelegate()
{
}

void SensorDelegate::buttonEventFired(ButtonEvent event)
{
	printf("\nButton Event Fired. ButtonEventType: %d from id: %d", event.buttonEventType, event.sender);
}

void SensorDelegate::pointerEventFired(PointerEvent event)
{
	printf("\nPointer Event Fired. Pointer X: %d, Pointer Y: %d from id: %d", event.x, event.y, event.sender);
}

void SensorDelegate::gestureEventFired(GestureEvent event)
{
	printf("\nGesture Event Fired. Gesture Type: %d from id: %d", event.gestureType, event.sender);
}

void SensorDelegate::pose6DEventFired(Pose6DEvent event)
{
	mMtx.lock();
// 		char temp[512];
// 		sprintf_s(temp, "vID: %d \n", event.sender);
// 		OutputDebugStringA(temp);

		//printf("\nPose6D Event Fired. Yaw: %f, Pitch: %f, Roll %f from id: %d", event.yaw, event.pitch, event.roll, event.sender);
		if (pose6DEventsMap[event.sender] != NULL)
		{
			pose6DEventsMap[event.sender]->pitch = event.pitch;
			pose6DEventsMap[event.sender]->yaw = event.yaw;
			pose6DEventsMap[event.sender]->roll = event.roll;
		}
	mMtx.unlock();
}