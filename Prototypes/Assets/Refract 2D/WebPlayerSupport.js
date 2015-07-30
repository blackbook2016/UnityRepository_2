#pragma strict

//This script is not needed as part of Refract 2D, it is only to help with presentation when building the example scenes as a webplayer
function Awake () {
    Application.targetFrameRate = 60;    // Make it run smoother in the webplayer
}

function Start () {
	Screen.SetResolution(Screen.currentResolution.width,Screen.currentResolution.height,false,60);	//Try to make it as high-res as the desktop, which is likely the highest res
}
