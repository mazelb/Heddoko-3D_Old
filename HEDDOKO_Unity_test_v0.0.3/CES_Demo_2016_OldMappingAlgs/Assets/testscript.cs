using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class testscript : MonoBehaviour
{
     
    public Text RefText;
	// Use this for initialization
	void Start ()
	{
	    
	}
	
	// Update is called once per frame
	void Update () {
       
    }

    void OnDrawGizmos()
    {
        GetComponent<Text>().resizeTextMaxSize = RefText.fontSize;
    }
}
