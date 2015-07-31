#pragma strict

//Copyright (c) 2013 Paul West/Venus12 LLC

//Version 1.1

var TitleImage : Texture2D;	//Title image for editor

var TextureToConvert : Texture2D;	//Texture to convert from height map to distortion map, Alpha8, RGB24, RGBA32 or ARGB32

var SlopeSamplingX : int=1;				//Rate of change adjustment in X
var SlopeSamplingY : int=1;				//Rate of change adjustment in Y
var MultiSample : boolean=true;			//Whether to average between 2 slopes or just use one
var SampleContrast : float=7;			//Scaling of the sampling (4-10 is good)
var AskForFilePath  : boolean=false;	//Whether to ask for a file path and filename in a window
var SampleWrapX : boolean=true;			//Whether to wrap sampling around left/right edges of source texture
var SampleWrapY : boolean=true;			//Whether to wrap sampling around top/bottom edges of source texture
var XSmoothing : int=4;					//How much gaussian smoothing to apply horizontally (in pixels)
var YSmoothing : int=4;					//How much gaussian smoothing to apply vertically (in pixels)
var XSmoothingWrap : boolean=true;		//Whether to wrap horizontal smoothing, otherwise clamp
var YSmoothingWrap : boolean=true;		//Whether to wrap vertical smoothing, otherwise clamp
