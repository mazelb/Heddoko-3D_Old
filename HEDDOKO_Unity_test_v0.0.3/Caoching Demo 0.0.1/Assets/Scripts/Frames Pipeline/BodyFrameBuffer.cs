using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils;

public class BodyFrameBuffer : CircularQueue<BodyFrame>
{


    public BodyFrameBuffer(int capacity)
        : base(capacity, true)
    {
        
    }

    public BodyFrameBuffer() : base()
    {
        
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
