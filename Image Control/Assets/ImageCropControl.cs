/*
 * 
 * The Control-Point Array Key Number Expressed As Follows :
 * 
 *                 0 ------ 1 ------ 2
 *                 |                 |
 *                 7      image      3
 *                 |                 |
 *                 6 ------ 5 ------ 4
 * 
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class ImageCropControl : MonoBehaviour {
	 
	public Texture2D controlPointImage;
	public GameObject controlPointRoot;
	public int controlPointWidth = 20, controlPointHeight = 20;
	public int minHeightLimit = 50, minWidthLimit = 50;
	[Range(0, 1)]
	public float moveAlpha = 0.5f;
	
	public Texture2D mouseDrag;
	public Texture2D mouseRL;
	public Texture2D mouseTB;
	public Texture2D mouse17CLK;
	public Texture2D mouse115CLK;

	RectTransform imgRT;

	GameObject [] controlPoint = new GameObject[8];
	Vector2 [] ControlPointPosition = new Vector2[8];
	
	public bool controlEnable = true;
	
	public bool lockR = false;
	public float lockValue = 0;
	
	void Awake () {
		ControlPointInit ();
		SetControlEnable (controlEnable);
		imgRT = GetComponent <RectTransform> ();
	}
	
	void ControlPointInit(){
		
		for(int i = 0; i < controlPoint.Length; i++){
			controlPoint[i] = new GameObject("Control Point " + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(EventTrigger));

			// ------------------------------- OnDrag
			
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener(OnDrag_ControlPoint);

			// ------------------------------- OnBeginDrag
			
			EventTrigger.Entry entry2 = new EventTrigger.Entry();
			entry2.eventID = EventTriggerType.BeginDrag;
			entry2.callback.AddListener(OnBeginDrag_ControlPoint);
			
			// ------------------------------- OnEndDrag
			
			EventTrigger.Entry entry3 = new EventTrigger.Entry();
			entry3.eventID = EventTriggerType.EndDrag;
			entry3.callback.AddListener(OnEndDrag_ControlPoint);

			// ----------------------------------- Add EventTrigger

			EventTrigger trigger = controlPoint[i].GetComponent<EventTrigger>();
			trigger.delegates = new List<EventTrigger.Entry>();
			trigger.delegates.Add(entry);
			trigger.delegates.Add(entry2);
			trigger.delegates.Add(entry3);
			// ----------------------------------- OnEnter

			EventTriggerListener.Get(controlPoint[i]).onEnter = ControlPointEnter;

			// -----------------------------------

			Transform transform = controlPoint[i].transform;
			transform.parent = controlPointRoot.transform;
			transform.localScale = Vector3.one;

			RawImage img = controlPoint[i].GetComponent<RawImage>();
			img.texture = controlPointImage;

			RectTransform rt = controlPoint[i].GetComponent<RectTransform>();
			rt.sizeDelta = new Vector2(controlPointWidth, controlPointHeight);

			EventTrigger et = controlPoint[i].GetComponent<EventTrigger>();

		}
		ControlPointPositionRefresh ();
	}

	public void ControlPointPositionRefresh(){

		RectTransform rt = GetComponent<RectTransform>();
		
		float X = rt.anchoredPosition.x;
		float Y = rt.anchoredPosition.y;
		
		float L = X - rt.sizeDelta.x / 2 - controlPointWidth;
		float R = X + rt.sizeDelta.x / 2;
		float T = Y + rt.sizeDelta.y / 2 + controlPointHeight * 1.25f;
		float B = Y - rt.sizeDelta.y / 2 + controlPointHeight * 0.25f; // controlPointHeight
		
		ControlPointPosition [0] = new Vector2 (L, T);
		ControlPointPosition [1] = new Vector2 (X-controlPointWidth/2, T);
		ControlPointPosition [2] = new Vector2 (R, T);
		ControlPointPosition [3] = new Vector2 (R, Y+controlPointHeight/2);
		ControlPointPosition [4] = new Vector2 (R, B);
		ControlPointPosition [5] = new Vector2 (X-controlPointWidth/2, B);
		ControlPointPosition [6] = new Vector2 (L, B);
		ControlPointPosition [7] = new Vector2 (L, Y+controlPointHeight/2);
		
		for(int i = 0; i < controlPoint.Length; i++){
			controlPoint[i].GetComponent<RectTransform>().anchoredPosition = ControlPointPosition[i];
		}
	}

	float tempDragMouseX;
	float tempDragMouseY;

	bool initDrag = true;

	public void OnUp(){
		initDrag = true;
	}

	public void OnDrag(){
		if(!controlEnable){
			return;
		}

		if(initDrag){
			initDrag = false;
			tempDragMouseX = Input.mousePosition.x;
			tempDragMouseY = Input.mousePosition.y;
		}

		float dragX = Input.mousePosition.x - tempDragMouseX;
		float dragY = Input.mousePosition.y - tempDragMouseY;

		RectTransform rt = GetComponent<RectTransform> ();

		float x = rt.anchoredPosition.x + dragX;
		float y = rt.anchoredPosition.y + dragY;

//		float x = Input.mousePosition.x - Screen.width / 2;
//		float y = Input.mousePosition.y - Screen.height / 2;


		rt.anchoredPosition = new Vector2 (x, y);



		tempDragMouseX = Input.mousePosition.x;
		tempDragMouseY = Input.mousePosition.y;

//		float x = img.transform.localPosition.x;
//		float y = img.transform.localPosition.y;
//		float speed = 1680 / (float)(Screen.width + Screen.height);
//		float imgX = x + dragVector.x * speed;
//		float imgY = y + dragVector.y * speed;
//		
//		if(lockR && imgX < 20){
//			return;
//		}
//		
//		// Move Image
//		SetXY (obj, imgX, imgY);
//		// Move Control Point
//		ControlPointPositionRefresh ();
//		// If Move Image, Use Alpha
//		obj.GetComponent<UITexture> ().alpha = moveAlpha;
//		foreach(GameObject cp in controlPoint){
//			cp.GetComponent<UITexture> ().alpha = moveAlpha;
//		}
	}

	// Change Mouse icon
	public void OnEnter(){
//		if(!controlEnable || isControlPointEnter){
//			return;
//		}
//		Cursor.SetCursor (mouseDrag, new Vector2(16,16), CursorMode.Auto);
	}

	// Change Mouse icon
	public void OnExit(){
		if(!controlEnable){
			return;
		}
		Cursor.SetCursor (null, new Vector2(16,16), CursorMode.Auto);
	}
	
	// Change Control-Point icon
	void ControlPointEnter(GameObject obj){
		switch (obj.name) {
		case "Control Point 3" :
		case "Control Point 7" :
			Cursor.SetCursor (mouseRL, new Vector2(16,16), CursorMode.Auto);
			break;
		case "Control Point 1" :
		case "Control Point 5" :
			Cursor.SetCursor (mouseTB, new Vector2(16,16), CursorMode.Auto);
			break;
		case "Control Point 2" :
		case "Control Point 6" :
			Cursor.SetCursor (mouse17CLK, new Vector2(16,16), CursorMode.Auto);
			break;
		case "Control Point 0" :
		case "Control Point 4" :
			Cursor.SetCursor (mouse115CLK, new Vector2(16,16), CursorMode.Auto);
			break;
		}
		
	}

	public void OnBeginDrag_ControlPoint (BaseEventData eventData){
		print ("--------------- Begin Drag");
	}
	
	public void OnEndDrag_ControlPoint (BaseEventData eventData){
		print ("--------------- End Drag");
		if(!controlEnable){
			return;
		}
		//		obj.GetComponent<UITexture> ().alpha = 1;
		//		foreach(GameObject cp in controlPoint){
		//			cp.GetComponent<UITexture> ().alpha = 1;
		//		}
	}


	
	//	float tempX = 0;
	
	void OnDrag_ControlPoint(BaseEventData data){

		PointerEventData p = (PointerEventData)data;
		Vector2 dragVector = p.delta;

		GameObject obj = p.pointerPress;

		RectTransform rt = GetComponent <RectTransform>();

//		Transform t = img.transform;
		float x = rt.anchoredPosition.x;
		float y = rt.anchoredPosition.y;
		int w = (int) rt.sizeDelta.x;
		int h = (int) rt.sizeDelta.y;

		// To Top
		if(obj.name == "Control Point 0" || obj.name == "Control Point 1" || obj.name == "Control Point 2"){
			float height = imgRT.sizeDelta.y + dragVector.y;
			if(height > minHeightLimit){
				Vector2 delta = new Vector2(0, p.delta.y);
				Vector2 deltaMove = delta / 2;
				imgRT.sizeDelta += delta;
				imgRT.anchoredPosition += deltaMove;
				// Top
				controlPoint[0].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[1].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[2].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				// Bottom
				controlPoint[4].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[5].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[6].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
			}
		}
		
		// To Right
		if(obj.name == "Control Point 2" || obj.name == "Control Point 3" || obj.name == "Control Point 4"){
			float width = imgRT.sizeDelta.x + dragVector.x;		
			if(width > minWidthLimit){
				Vector2 delta = new Vector2(p.delta.x, 0);
				Vector2 deltaMove = delta / 2;
				imgRT.sizeDelta += delta;
				imgRT.anchoredPosition += deltaMove;
				// Right
				controlPoint[2].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[3].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[4].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				// Left
				controlPoint[0].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[6].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[7].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
			}
		}
	
		// To Bottom
		if(obj.name == "Control Point 4" || obj.name == "Control Point 5" || obj.name == "Control Point 6"){
			float height = imgRT.sizeDelta.y + dragVector.y;
			if(height > minHeightLimit){
				Vector2 delta = new Vector2(0, - p.delta.y);
				Vector2 deltaMove = delta / 2;
				imgRT.sizeDelta += delta;
				imgRT.anchoredPosition -= deltaMove;
				// Bottom
				controlPoint[4].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[5].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[6].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				// Top
				controlPoint[0].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[1].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[2].GetComponent<RectTransform>().anchoredPosition += deltaMove;
			}
		}
		
		// To Left
		if(obj.name == "Control Point 6" || obj.name == "Control Point 7" || obj.name == "Control Point 0"){
			float width = imgRT.sizeDelta.x + dragVector.x;		
			if(width > minWidthLimit){
				Vector2 delta = new Vector2( - p.delta.x, 0);
				Vector2 deltaMove = delta / 2;
				imgRT.sizeDelta += delta;
				imgRT.anchoredPosition -= deltaMove;
				// Left
				controlPoint[2].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[3].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				controlPoint[4].GetComponent<RectTransform>().anchoredPosition += deltaMove;
				// Right
				controlPoint[0].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[6].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
				controlPoint[7].GetComponent<RectTransform>().anchoredPosition -= deltaMove;
			}
		}
	}
	
	public void SetControlEnable(bool value){
		controlEnable = value;
//		foreach (GameObject obj in controlPoint) {
//			obj.SetActive(value);
//		}
	}
		
	void SetX(GameObject obj, float x){
		RectTransform rt = obj.GetComponent<RectTransform> ();
		rt.anchoredPosition =  new Vector3 (x, rt.anchoredPosition.y);
	}
	
	void SetY(GameObject obj, float y){
		obj.transform.localPosition =  new Vector3 (obj.transform.localPosition.x, y);
	}
	
	void SetXY(GameObject obj, float x, float y){
		obj.transform.localPosition =  new Vector3 (x, y);
	}
	
}
