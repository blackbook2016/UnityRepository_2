import System.IO;

@CustomEditor (HeightMapTool)

//Editor for HeightMapTool - Copyright (c) 2012 Paul West/Venus12 LLC
//Version 1.1

class HeightMapToolEditor extends Editor {
	//Editor for the HeightMapTool script

	var Tex:Texture2D;	//Temporary
	
	function OnInspectorGUI () {
		//Generate and operate the editor
		
		//Get relative top of inspector area
		EditorGUILayout.Space();		//Create a space, relative to the top of the script section
		if(Event.current.type == EventType.Repaint) { 		//When it repaints
			TopLeftX = GUILayoutUtility.GetLastRect().x; 	//Get the rect of the space, so we have relative coordinates for the top left of the GUI
			TopLeftY = GUILayoutUtility.GetLastRect().y;	//Otherwise EditorGUI would be relative to the very top of the inspector window
		}

		//Show title
		EditorGUIUtility.LookLikeControls(430,0);
		EditorGUI.LabelField(Rect(TopLeftX,TopLeftY+40,430,20),"HeightMap Converter Tool","");
		EditorGUI.LabelField(Rect(TopLeftX+35,TopLeftY+60,430,20),"Copyright (c) 2012 Paul West/Venus12 LLC","");
		EditorGUI.DrawPreviewTexture(Rect(TopLeftX,TopLeftY+10,375,50),target.TitleImage,null,ScaleMode.ScaleToFit);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUIUtility.LookLikeControls(230,130);
		
		//Texture to convert
		target.TextureToConvert=EditorGUILayout.ObjectField(GUIContent("Height Map Texture","Alpha8, RGB24, RGBA32 or ARGB32 texture with Read/Write Enabled. Should contain grayscale (Red=Height or Alplha8=Height), with optional Alpha channel"),target.TextureToConvert,Texture2D,true);
		Tex=target.TextureToConvert as Texture2D;
		if (Tex) {
			if ((Tex.format==TextureFormat.Alpha8) || (Tex.format==TextureFormat.RGB24) || (Tex.format==TextureFormat.RGBA32) || (Tex.format==TextureFormat.ARGB32)) {
				//Texture format is ok
			} else {
				//Texture format is not allowed
				EditorUtility.DisplayDialog("Height Map Tool","Sorry but this texture is not in one of the accepted formats. Height Map Tool requires either an Alpha8, RGB24, RGBA32 or ARGB32 texture format. Please convert your texture to one of these formats in the texture's Import settings or choose another texture.","Ok");
				target.TextureToConvert=null;				
			}
		}
		
		//Slope interpretation
		target.SlopeSamplingX=EditorGUILayout.IntField(GUIContent("Slope Sampling X","Distance from current pixel in X at which to sample the heightmap slope. Use 1 for the adjacent pixel. Sampling further from the pixel works best with smooth height maps."),target.SlopeSamplingX);	//Slope offset X
		if (target.SlopeSamplingX<1) {target.SlopeSamplingX=1;}
		target.SlopeSamplingY=EditorGUILayout.IntField(GUIContent("Slope Sampling Y","Distance from current pixel in Y at which to sample the heightmap slope. Use 1 for the adjacent pixel. Sampling further from the pixel works best with smooth height maps."),target.SlopeSamplingY);	//Slope offset Y
		if (target.SlopeSamplingY<1) {target.SlopeSamplingY=1;}
		
		//Slope average
		target.MultiSample=EditorGUILayout.Toggle(GUIContent("Multi-Sampling","Whether to find the slope of two pixels or average across two adjacent slopes via multiple samples"),target.MultiSample);
		
		//Sample scale
		target.SampleContrast=EditorGUILayout.FloatField(GUIContent("Sample Contrast","Scale factor of the sampling, which adjusts the contrast/dynamic range. Values in the range 4..10 usually work well, lower values have a wider dynamic range, higher values have a narrower range"),target.SampleContrast);
		if (target.SampleContrast<0.01) {target.SampleContrast=0.01;}
		
		//Wrap or clamp sampling
		target.SampleWrapX=EditorGUILayout.Toggle(GUIContent("Wrap X Sampling","Whether to allow the sampling of the source texture to wrap around horizontally at the left and right edges, or clamp it"),target.SampleWrapX);
		target.SampleWrapY=EditorGUILayout.Toggle(GUIContent("Wrap Y Sampling","Whether to allow the sampling of the source texture to wrap around vertically at the top and bottom edges, or clamp it"), target.SampleWrapY);
		
		//Smoothing
		target.XSmoothing=EditorGUILayout.IntField(GUIContent("X Smoothing","How much gaussian smoothing to apply horizontally after the refraction map has been calculated. Use this to smooth out graininess. Measured in pixels, higher values blur more"),target.XSmoothing);
		if (target.XSmoothing<0) {target.XSmoothing=0;}
		target.YSmoothing=EditorGUILayout.IntField(GUIContent("Y Smoothing","How much gaussian smoothing to apply vertically after the refraction map has been calculated. Use this to smooth out graininess. Measured in pixels, higher values blur more"),target.YSmoothing);
		if (target.YSmoothing<0) {target.YSmoothing=0;}

		//Wrap or clamp sampling
		target.XSmoothingWrap=EditorGUILayout.Toggle(GUIContent("Wrap X Smoothing","Whether to allow the horizontal smoothing to wrap around at the left and right edges, or clamp it"),target.XSmoothingWrap);
		target.YSmoothingWrap=EditorGUILayout.Toggle(GUIContent("Wrap Y Smoothing","Whether to allow the vertical smoothing to wrap around at the top and bottom edges, or clamp it"), target.YSmoothingWrap);

		//Ask for file path
		target.AskForFilePath=EditorGUILayout.Toggle(GUIContent("Ask for File Path","Whether to show a window where you can choose the file name and path to save the output texture"),target.AskForFilePath);
		
		//Convert button
		//Conversion will not work if the source texture is not marked as Read/Write Enabled
		if (GUILayout.Button("Convert to Distortion Map")) {
			if (Tex){
				var path : String;
				if (target.AskForFilePath==true) {
					//Ask for file path and name
					path = EditorUtility.SaveFilePanelInProject("Save Distortion Map as PNG",Tex.name + ".png","png","Please enter a file name to save the texture to");
		        } else {
		        	//Assume file path should be same as original texture with same name + `-Distortion.png`
		        	path = AssetDatabase.GetAssetPath(Tex);
		        	path=path.Substring(0,path.Length-4);	//Strip off dot plus 3-character extension
		        	path+="-DistortionMap.png";
		        }
		        if(path.Length != 0) {
		        	//Make sure the texture isReadable
		        	var temppath:String=AssetDatabase.GetAssetPath(Tex);	//Get the full path + filename of the original texture
			        var textureImporter:TextureImporter = AssetImporter.GetAtPath(temppath) as TextureImporter;
					if (textureImporter.isReadable==false) {
						//Is not readable, make it so!
			       		if (EditorUtility.DisplayDialog("Height Map Tool","Height Map Tool requires that your original texture have the `Read/Write Enabled` flag set in your texture's Import settings. Your texture: `"+Tex.name+"` does not have it set. Is it okay to go ahead and change it?","Yes Please","No Thanks")==false) {
			       			return;		//Quit!
			       		}
						textureImporter.isReadable=true;		//Make it readable, we must be able to access it
			       		AssetDatabase.ImportAsset(temppath);	//Reimport it with new settings
					}

		        	//Convert it
					if (Tex.format==TextureFormat.Alpha8) {
						ConvertHeightMap(0,path);	//Convert from Alpha8
					} else {
						if (Tex.format==TextureFormat.RGB24) {
							ConvertHeightMap(1,path);	//Convert from RGB24
						} else {
							if (Tex.format==TextureFormat.RGBA32) {
								ConvertHeightMap(2,path);	//Convert from RGBA32 with alpha
							} else {
								if (Tex.format==TextureFormat.ARGB32) {
									ConvertHeightMap(3,path);	//Convert from ARGB32 with alpha
								}
							}
						}
					}
				}
			} else {
				//There is no texture selected
				EditorUtility.DisplayDialog("Height Map Tool","Please drag a Texture onto the Height Map Texture box, or click on the small `Select` button within that box to choose a texture. Acceptable texture formats are Alpha8, RGB24, RGBA32 and ARGB32.","Ok");
			}
		}
	}

	function ConvertHeightMap(Type:int,FilePath:String) {
		//Convert the texture from a grayscale heightmap (red channel) to a distortion map
		//Distortion map is an RGB uncompressed texture where:
		//R=XOffset in a 128..-127 range (as 0..1)
		//G=YOffset in a 128..-127 range (as 0..1)
		//B=Unused
		//A=Alpha channel from original texture
		//Type defines the source texture type, 0=Alpha8, 1=RGB24, 2=RGBA32 with alpha channel
		//Outputs RGB24 texture for Alpha8/RGB24 input, or RGBA32 texture for RGBA32 input
		//FilePath is the path and filename of the file (.png)
		var Pixels:Color32[];
		Pixels=Tex.GetPixels32(0);		//Get pixels from texture, RGBA
		var DistPixels:Color[];
		DistPixels=new Color[Tex.width*Tex.height];	//Create new space
		var DistortionMap:Texture2D;	//Output texture
		var Y:int;
		var X:int;
		var Temp:int;
		var Height:int;
		var Height2:int;
		var Height3:int;
		var SlopeA:float;
		var SlopeB:float;
		var Normal:float;
		var AngleA:float;
		var AngleB:float;
		var Offset:float;
		
		//Progress bar
		EditorUtility.DisplayCancelableProgressBar("Height Map Tool","Generating Distortion Map",0);
		
		//Convert to distortion map
		if (Type==0) {
			//Alpha8 grayscale, Alpha channel is height
			DistortionMap = Texture2D(Tex.width,Tex.height,TextureFormat.RGB24,false);	//Create non-mipmapped RGB texture
			//Calculate X offset
			for (Y=0; Y<Tex.height; Y++){
				for (X=0; X<Tex.width; X++){
					Height=Pixels[(Y*Tex.width)+X].a;		//Get this pixel's height, 0..255
					Temp=X-target.SlopeSamplingX;			//Offset sampling
					if (target.SampleWrapX==true) {
						if (Temp<0) {Temp+=Tex.width;}		//Wrap left
					} else {
						if (Temp<0) {Temp=0;}				//Clamp left
					}
					Height2=Pixels[(Y*Tex.width)+Temp].a;	//Get height of pixel to left
					Temp=X+target.SlopeSamplingX;			//Offset sampling
					if (target.SampleWrapX==true) {
						if (Temp>=Tex.width) {Temp-=Tex.width;}	//Wrap right
					} else {
						if (Temp>=Tex.width) {Temp=Tex.width-1;}//Clamp right
					}
					Height3=Pixels[(Y*Tex.width)+Temp].a;	//Get height of pixel to right
					SlopeA=Height-Height2;					//Slope to left
					SlopeB=Height3-Height;					//Slope to right
					if (target.MultiSample==true){
						//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
						AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
						AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
						AngleA=(AngleA+AngleB)/2.0;			//Average angle
						Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
						Normal=(Offset/target.SampleContrast)+0.5;
					} else {
						//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
						AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
						Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
						Normal=(Offset/target.SampleContrast)+0.5;
					}
					DistPixels[(Y*Tex.width)+X].r=Mathf.Min(Normal,1.0);	//Set red as X offset
				}
				//Progress bar
				if (Y % 16==0) {
					if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Horizontal Distortion... "+(((1.0*Y)/Tex.height)*25)+"%",0.25*((1.0*Y)/Tex.height))) {	//Display progress bar
						//User cancelled the operation, quit!
						EditorUtility.ClearProgressBar();			//Remove the progress bar
						AssetDatabase.Refresh();					//Do a final refresh
						EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
						return;
					}
				}
			}
			//Calculate Y offset
			for (X=0; X<Tex.width; X++){
				for (Y=0; Y<Tex.height; Y++){
					Height=Pixels[(Y*Tex.width)+X].a;		//Get this pixel's height, 0..255
					Temp=Y-target.SlopeSamplingY;			//Offset sampling
					if (target.SampleWrapY==true) {
						if (Temp<0) {Temp+=Tex.height;}		//Wrap top
					} else {
						if (Temp<0) {Temp=0;}				//Clamp top
					}
					Height2=Pixels[(Temp*Tex.width)+X].a;	//Get height of pixel to left
					Temp=Y+target.SlopeSamplingY;			//Offset sampling
					if (target.SampleWrapY==true) {
						if (Temp>=Tex.height) {Temp-=Tex.height;}	//Wrap bottom
					} else {
						if (Temp>=Tex.height) {Temp=Tex.height-1;}	//Clamp bottom
					}
					Height3=Pixels[(Temp*Tex.width)+X].a;	//Get height of pixel to right
					SlopeA=Height-Height2;					//Slope to left
					SlopeB=Height3-Height;					//Slope to right
					if (target.MultiSample==true){
						//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
						AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
						AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
						AngleA=(AngleA+AngleB)/2.0;			//Average angle
						Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
						Normal=(Offset/target.SampleContrast)+0.5;	
					} else {
						//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
						AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
						Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
						Normal=(Offset/target.SampleContrast)+0.5;
					}
					DistPixels[(Y*Tex.width)+X].g=Mathf.Min(Normal,1.0);	//Set green as Y offset
				}
				//Progress bar
				if (X % 16==0) {
					if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Vertical Distortion... "+(((1.0*X)/Tex.width)*25+25)+"%",0.25+0.25*((1.0*X)/Tex.width))) {	//Display progress bar
						//User cancelled the operation, quit!
						EditorUtility.ClearProgressBar();			//Remove the progress bar
						AssetDatabase.Refresh();					//Do a final refresh
						EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
						return;
					}
				}
			}
		
		} else {
			if (Type==1) {
				//RGB24 grayscale, Red channel is height
				DistortionMap = Texture2D(Tex.width,Tex.height,TextureFormat.RGB24,false);	//Create non-mipmapped RGB texture
				//Calculate X offset
				for (Y=0; Y<Tex.height; Y++){
					for (X=0; X<Tex.width; X++){
						Height=Pixels[(Y*Tex.width)+X].r;		//Get this pixel's height, 0..255
						Temp=X-target.SlopeSamplingX;			//Offset sampling
						if (target.SampleWrapX==true) {
							if (Temp<0) {Temp+=Tex.width;}		//Wrap left
						} else {
							if (Temp<0) {Temp=0;}				//Clamp left
						}
						Height2=Pixels[(Y*Tex.width)+Temp].r;	//Get height of pixel to left
						Temp=X+target.SlopeSamplingX;			//Offset sampling
						if (target.SampleWrapX==true) {
							if (Temp>=Tex.width) {Temp-=Tex.width;}	//Wrap right
						} else {
							if (Temp>=Tex.width) {Temp=Tex.width-1;}//Clamp right
						}
						Height3=Pixels[(Y*Tex.width)+Temp].r;	//Get height of pixel to right
						SlopeA=Height-Height2;					//Slope to left
						SlopeB=Height3-Height;					//Slope to right
						if (target.MultiSample==true){
							//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
							AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
							AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
							AngleA=(AngleA+AngleB)/2.0;			//Average angle
							Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
							Normal=(Offset/target.SampleContrast)+0.5;
						} else {
							//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
							AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
							Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
							Normal=(Offset/target.SampleContrast)+0.5;
						}
						DistPixels[(Y*Tex.width)+X].r=Mathf.Min(Normal,1.0);	//Set red as X offset
					}
					//Progress bar
					if (Y % 16==0) {
						if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Horizontal Distortion... "+(((1.0*Y)/Tex.height)*25)+"%",0.25*((1.0*Y)/Tex.height))) {	//Display progress bar
							//User cancelled the operation, quit!
							EditorUtility.ClearProgressBar();			//Remove the progress bar
							AssetDatabase.Refresh();					//Do a final refresh
							EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
							return;
						}
					}
				}
				//Calculate Y offset
				for (X=0; X<Tex.width; X++){
					for (Y=0; Y<Tex.height; Y++){
						Height=Pixels[(Y*Tex.width)+X].r;		//Get this pixel's height, 0..255
						Temp=Y-target.SlopeSamplingY;			//Offset sampling
						if (target.SampleWrapY==true) {
							if (Temp<0) {Temp+=Tex.height;}		//Wrap top
						} else {
							if (Temp<0) {Temp=0;}				//Clamp top
						}
						Height2=Pixels[(Temp*Tex.width)+X].r;	//Get height of pixel to left
						Temp=Y+target.SlopeSamplingY;			//Offset sampling
						if (target.SampleWrapY==true) {
							if (Temp>=Tex.height) {Temp-=Tex.height;}	//Wrap bottom
						} else {
							if (Temp>=Tex.height) {Temp=Tex.height-1;}	//Clamp bottom
						}
						Height3=Pixels[(Temp*Tex.width)+X].r;	//Get height of pixel to right
						SlopeA=Height-Height2;					//Slope to left
						SlopeB=Height3-Height;					//Slope to right
						if (target.MultiSample==true){
							//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
							AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
							AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
							AngleA=(AngleA+AngleB)/2.0;			//Average angle
							Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
							Normal=(Offset/target.SampleContrast)+0.5;	
						} else {
							//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
							AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
							Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
							Normal=(Offset/target.SampleContrast)+0.5;
						}
						DistPixels[(Y*Tex.width)+X].g=Mathf.Min(Normal,1.0);	//Set green as Y offset
					}
					//Progress bar
					if (X % 16==0) {
						if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Vertical Distortion... "+(((1.0*X)/Tex.width)*25+25)+"%",0.25+0.25*((1.0*X)/Tex.width))) {	//Display progress bar
							//User cancelled the operation, quit!
							EditorUtility.ClearProgressBar();			//Remove the progress bar
							AssetDatabase.Refresh();					//Do a final refresh
							EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
							return;
						}
					}
				}
			
			} else {
				if (Type==2) {
					//RGBA32 grayscale red channel is height, Alpha channel is alpha
					DistortionMap = Texture2D(Tex.width,Tex.height,TextureFormat.ARGB32,false);	//Create non-mipmapped ARGB32 texture
					//Calculate X offset
					for (Y=0; Y<Tex.height; Y++){
						for (X=0; X<Tex.width; X++){
							Height=Pixels[(Y*Tex.width)+X].r;		//Get this pixel's height, 0..255
							Temp=X-target.SlopeSamplingX;			//Offset sampling
							if (target.SampleWrapX==true) {
								if (Temp<0) {Temp+=Tex.width;}		//Wrap left
							} else {
								if (Temp<0) {Temp=0;}				//Clamp left
							}
							Height2=Pixels[(Y*Tex.width)+Temp].r;	//Get height of pixel to left
							Temp=X+target.SlopeSamplingX;			//Offset sampling
							if (target.SampleWrapX==true) {
								if (Temp>=Tex.width) {Temp-=Tex.width;}	//Wrap right
							} else {
								if (Temp>=Tex.width) {Temp=Tex.width-1;}//Clamp right
							}
							Height3=Pixels[(Y*Tex.width)+Temp].r;	//Get height of pixel to right
							SlopeA=Height-Height2;					//Slope to left
							SlopeB=Height3-Height;					//Slope to right
							if (target.MultiSample==true){
								//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
								AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
								AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
								AngleA=(AngleA+AngleB)/2.0;			//Average angle
								Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
								Normal=(Offset/target.SampleContrast)+0.5;
							} else {
								//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
								AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
								Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
								Normal=(Offset/target.SampleContrast)+0.5;
							}
							DistPixels[(Y*Tex.width)+X].r=Mathf.Min(Normal,1.0);	//Set red as X offset
						}
						//Progress bar
						if (Y % 16==0) {
							if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Horizontal Distortion... "+(((1.0*Y)/Tex.height)*25)+"%",0.25*((1.0*Y)/Tex.height))) {	//Display progress bar
								//User cancelled the operation, quit!
								EditorUtility.ClearProgressBar();			//Remove the progress bar
								AssetDatabase.Refresh();					//Do a final refresh
								EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
								return;
							}
						}
					}
					//Calculate Y offset
					for (X=0; X<Tex.width; X++){
						for (Y=0; Y<Tex.height; Y++){
							Height=Pixels[(Y*Tex.width)+X].r;		//Get this pixel's height, 0..255
							Temp=Y-target.SlopeSamplingY;			//Offset sampling
							if (target.SampleWrapY==true) {
								if (Temp<0) {Temp+=Tex.height;}		//Wrap top
							} else {
								if (Temp<0) {Temp=0;}				//Clamp top
							}
							Height2=Pixels[(Temp*Tex.width)+X].r;	//Get height of pixel to left
							Temp=Y+target.SlopeSamplingY;			//Offset sampling
							if (target.SampleWrapY==true) {
								if (Temp>=Tex.height) {Temp-=Tex.height;}	//Wrap bottom
							} else {
								if (Temp>=Tex.height) {Temp=Tex.height-1;}	//Clamp bottom
							}
							Height3=Pixels[(Temp*Tex.width)+X].r;	//Get height of pixel to right
							SlopeA=Height-Height2;					//Slope to left
							SlopeB=Height3-Height;					//Slope to right
							if (target.MultiSample==true){
								//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
								AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
								AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
								AngleA=(AngleA+AngleB)/2.0;			//Average angle
								Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
								Normal=(Offset/target.SampleContrast)+0.5;	
							} else {
								//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
								AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
								Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
								Normal=(Offset/target.SampleContrast)+0.5;
							}
							DistPixels[(Y*Tex.width)+X].g=Mathf.Min(Normal,1.0);	//Set green as Y offset
							DistPixels[(Y*Tex.width)+X].a=Pixels[(Y*Tex.width)+X].a;	//Transfer source alpha to dest
						}
						//Progress bar
						if (X % 16==0) {
							if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Vertical Distortion... "+(((1.0*X)/Tex.width)*25+25)+"%",0.25+0.25*((1.0*X)/Tex.width))) {	//Display progress bar
								//User cancelled the operation, quit!
								EditorUtility.ClearProgressBar();			//Remove the progress bar
								AssetDatabase.Refresh();					//Do a final refresh
								EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
								return;
							}
						}
					}
				
				} else {
					if (Type==3) {
						//ARGB32 grayscale red channel is height, Alpha channel is alpha
						DistortionMap = Texture2D(Tex.width,Tex.height,TextureFormat.ARGB32,false);	//Create non-mipmapped ARGB32 texture
						//Calculate X offset
						for (Y=0; Y<Tex.height; Y++){
							for (X=0; X<Tex.width; X++){
								Height=Pixels[(Y*Tex.width)+X].r;		//Get this pixel's height, 0..255
								Temp=X-target.SlopeSamplingX;			//Offset sampling
								if (target.SampleWrapX==true) {
									if (Temp<0) {Temp+=Tex.width;}		//Wrap left
								} else {
									if (Temp<0) {Temp=0;}				//Clamp left
								}
								Height2=Pixels[(Y*Tex.width)+Temp].r;	//Get height of pixel to left
								Temp=X+target.SlopeSamplingX;			//Offset sampling
								if (target.SampleWrapX==true) {
									if (Temp>=Tex.width) {Temp-=Tex.width;}	//Wrap right
								} else {
									if (Temp>=Tex.width) {Temp=Tex.width-1;}//Clamp right
								}
								Height3=Pixels[(Y*Tex.width)+Temp].r;	//Get height of pixel to right
								SlopeA=Height-Height2;					//Slope to left
								SlopeB=Height3-Height;					//Slope to right
								if (target.MultiSample==true){
									//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
									AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
									AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
									AngleA=(AngleA+AngleB)/2.0;			//Average angle
									Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
									Normal=(Offset/target.SampleContrast)+0.5;
								} else {
									//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
									AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
									Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
									Normal=(Offset/target.SampleContrast)+0.5;
								}
								DistPixels[(Y*Tex.width)+X].r=Mathf.Min(Normal,1.0);	//Set red as X offset
							}
							//Progress bar
							if (Y % 16==0) {
								if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Horizontal Distortion... "+(((1.0*Y)/Tex.height)*25)+"%",0.25*((1.0*Y)/Tex.height))) {	//Display progress bar
									//User cancelled the operation, quit!
									EditorUtility.ClearProgressBar();			//Remove the progress bar
									AssetDatabase.Refresh();					//Do a final refresh
									EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
									return;
								}
							}
						}
						//Calculate Y offset
						for (X=0; X<Tex.width; X++){
							for (Y=0; Y<Tex.height; Y++){
								Height=Pixels[(Y*Tex.width)+X].r;		//Get this pixel's height, 0..255
								Temp=Y-target.SlopeSamplingY;			//Offset sampling
								if (target.SampleWrapY==true) {
									if (Temp<0) {Temp+=Tex.height;}		//Wrap top
								} else {
									if (Temp<0) {Temp=0;}				//Clamp top
								}
								Height2=Pixels[(Temp*Tex.width)+X].r;	//Get height of pixel to left
								Temp=Y+target.SlopeSamplingY;			//Offset sampling
								if (target.SampleWrapY==true) {
									if (Temp>=Tex.height) {Temp-=Tex.height;}	//Wrap bottom
								} else {
									if (Temp>=Tex.height) {Temp=Tex.height-1;}	//Clamp bottom
								}
								Height3=Pixels[(Temp*Tex.width)+X].r;	//Get height of pixel to right
								SlopeA=Height-Height2;					//Slope to left
								SlopeB=Height3-Height;					//Slope to right
								if (target.MultiSample==true){
									//Normal=((SlopeA+SlopeB)/511.0)+0.5;	//Average two slopes to find normal
									AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
									AngleB=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeB,256.0))))-90.0;	//Get angle of SlopeB -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
									AngleA=(AngleA+AngleB)/2.0;			//Average angle
									Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
									Normal=(Offset/target.SampleContrast)+0.5;	
								} else {
									//Normal=(SlopeA/255.0)+0.5;			//Find normal of one slope
									AngleA=(90.0-(Mathf.Rad2Deg*(Mathf.Atan2(SlopeA,256.0))))-90.0;	//Get angle of SlopeA -90 to 90 where 0 is straight upwards on Y axis, -90 to left, 90 to right
									Offset=LineIntersect(0,0,1000.0*Mathf.Cos((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),1000*Mathf.Sin((Mathf.Deg2Rad*AngleA)-(Mathf.Deg2Rad*90.0)),-1000,256,1000);
									Normal=(Offset/target.SampleContrast)+0.5;
								}
								DistPixels[(Y*Tex.width)+X].g=Mathf.Min(Normal,1.0);	//Set green as Y offset
								DistPixels[(Y*Tex.width)+X].a=Pixels[(Y*Tex.width)+X].a;	//Transfer source alpha to dest
							}
							//Progress bar
							if (X % 16==0) {
								if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Vertical Distortion... "+(((1.0*X)/Tex.width)*25+25)+"%",0.25+0.25*((1.0*X)/Tex.width))) {	//Display progress bar
									//User cancelled the operation, quit!
									EditorUtility.ClearProgressBar();			//Remove the progress bar
									AssetDatabase.Refresh();					//Do a final refresh
									EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
									return;
								}
							}
						}
					}
				}
			}
		}

		//Apply smoothing
		if (target.XSmoothing>0 || target.YSmoothing>0) {
			//Do some smoothing - applies a gaussian blur in separate horizontal and vertical passes
			//Red/X channel is processed separately from Green/Y channel
			var DistPixels2:Color[];
			DistPixels2=new Color[Tex.width*Tex.height];	//Create new space
			var BlurTotal:float;
			var BlurX:int;
			var BlurY:int;
			var BlurCenter:int;
			//Horizontal
			if (target.XSmoothing>0) {
				//Do horizontal gaussian blur - in-place
				//Red component
				BlurCenter=target.XSmoothing/2;				//Offset to get to center of blur
				for (Y=0; Y<Tex.height; Y++) {
					for (X=0; X<Tex.width; X++) {
						//Blur one pixel
						BlurTotal=0;
						for (BlurX=-target.XSmoothing; BlurX<=0; BlurX++) {
							Temp=BlurX+BlurCenter;			//Left edge of blur sampling, centered over pixel
							if (target.XSmoothingWrap==true) {
								if (X+Temp<0) {Temp+=Tex.width;}			//Wrap X left
								if (X+Temp>=Tex.width) {Temp-=Tex.width;}	//Wrap X right
							} else {
								if (X+Temp<0) {Temp=X;}						//Clamp X left
								if (X+Temp>=Tex.width) {Temp=(Tex.width-1)-X;}//Clamp X right
							}
							BlurTotal+=DistPixels[(Y*Tex.width)+X+Temp].r;	//Add red to total
						}
						BlurTotal/=(1.0*target.XSmoothing);	//Find average
						DistPixels2[(Y*Tex.width)+X].r=BlurTotal;	//Store blurred pixel
					}
					//Progress bar
					if (Y % 16==0) {
						if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Horizontal Smoothing... "+(((1.0*Y)/Tex.height)*12.5+50)+"%",0.5+0.125*((1.0*Y)/Tex.height))) {	//Display progress bar
							//User cancelled the operation, quit!
							EditorUtility.ClearProgressBar();			//Remove the progress bar
							AssetDatabase.Refresh();					//Do a final refresh
							EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
							return;
						}
					}
				}
				//Green component
				BlurCenter=target.XSmoothing/2;				//Offset to get to center of blur
				for (Y=0; Y<Tex.height; Y++) {
					for (X=0; X<Tex.width; X++) {
						//Blur one pixel
						BlurTotal=0;
						for (BlurX=-target.XSmoothing; BlurX<=0; BlurX++) {
							Temp=BlurX+BlurCenter;			//Left edge of blur sampling, centered over pixel
							if (target.XSmoothingWrap==true) {
								if (X+Temp<0) {Temp+=Tex.width;}			//Wrap X left
								if (X+Temp>=Tex.width) {Temp-=Tex.width;}	//Wrap X right
							} else {
								if (X+Temp<0) {Temp=X;}						//Clamp X left
								if (X+Temp>=Tex.width) {Temp=(Tex.width-1)-X;}//Clamp X right
							}
							BlurTotal+=DistPixels[(Y*Tex.width)+X+Temp].g;	//Add green to total
						}
						BlurTotal/=(1.0*target.XSmoothing);	//Find average
						DistPixels2[(Y*Tex.width)+X].g=BlurTotal;	//Store blurred pixel
					}
					//Progress bar
					if (Y % 16==0) {
						if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Horizontal Smoothing... "+(((1.0*Y)/Tex.height)*12.5+62.5)+"%",0.625+0.125*((1.0*Y)/Tex.height))) {	//Display progress bar
							//User cancelled the operation, quit!
							EditorUtility.ClearProgressBar();			//Remove the progress bar
							AssetDatabase.Refresh();					//Do a final refresh
							EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
							return;
						}
					}
				}
				//Copy back (let alpha remain if present)
				for (Y=0; Y<Tex.height; Y++) {
					for (X=0; X<Tex.width; X++) {
						DistPixels[(Y*Tex.width)+X].r=DistPixels2[(Y*Tex.width)+X].r;	//Copy blurred red
						DistPixels[(Y*Tex.width)+X].g=DistPixels2[(Y*Tex.width)+X].g;	//Copy blurred green
					}
				}
			}
			
			//Vertical
			if (target.YSmoothing>0) {
				//Do vertical gaussian blur - in-place
				//Red component
				BlurCenter=target.YSmoothing/2;				//Offset to get to center of blur
				for (X=0; X<Tex.width; X++) {
					for (Y=0; Y<Tex.height; Y++) {
						//Blur one pixel
						BlurTotal=0;
						for (BlurY=-target.YSmoothing; BlurY<=0; BlurY++) {
							Temp=BlurY+BlurCenter;			//Top edge of blur sampling, centered over pixel
							if (target.YSmoothingWrap==true) {
								if (Y+Temp<0) {Temp+=Tex.height;}			//Wrap Y top
								if (Y+Temp>=Tex.height) {Temp-=Tex.height;}	//Wrap Y bottom
							} else {
								if (Y+Temp<0) {Temp=Y;}						//Clamp Y top
								if (Y+Temp>=Tex.height) {Temp=(Tex.height-1)-Y;}//Clamp Y bottom
							}
							BlurTotal+=DistPixels[((Y+Temp)*Tex.width)+X].r;	//Add red to total
						}
						BlurTotal/=(1.0*target.YSmoothing);	//Find average
						DistPixels2[(Y*Tex.width)+X].r=BlurTotal;	//Store blurred pixel
					}
					//Progress bar
					if (X % 16==0) {
						if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Vertical Smoothing... "+(((1.0*X)/Tex.width)*12.5+75)+"%",0.75+0.125*((1.0*X)/Tex.width))) {	//Display progress bar
							//User cancelled the operation, quit!
							EditorUtility.ClearProgressBar();			//Remove the progress bar
							AssetDatabase.Refresh();					//Do a final refresh
							EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
							return;
						}
					}
				}
				//Green component
				BlurCenter=target.YSmoothing/2;				//Offset to get to center of blur
				for (X=0; X<Tex.width; X++) {
					for (Y=0; Y<Tex.height; Y++) {
						//Blur one pixel
						BlurTotal=0;
						for (BlurY=-target.YSmoothing; BlurY<=0; BlurY++) {
							Temp=BlurY+BlurCenter;			//Top edge of blur sampling, centered over pixel
							if (target.YSmoothingWrap==true) {
								if (Y+Temp<0) {Temp+=Tex.height;}			//Wrap Y top
								if (Y+Temp>=Tex.height) {Temp-=Tex.height;}	//Wrap Y bottom
							} else {
								if (Y+Temp<0) {Temp=Y;}						//Clamp Y top
								if (Y+Temp>=Tex.height) {Temp=Tex.height-1-Y;}//Clamp Y bottom
							}
							BlurTotal+=DistPixels[((Y+Temp)*Tex.width)+X].g;	//Add green to total
						}
						BlurTotal/=(1.0*target.YSmoothing);	//Find average
						DistPixels2[(Y*Tex.width)+X].g=BlurTotal;	//Store blurred pixel
					}
					//Progress bar
					if (X % 16==0) {
						if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Vertical Smoothing... "+(((1.0*X)/Tex.width)*12.5+87.5)+"%",0.875+0.125*((1.0*X)/Tex.width))) {	//Display progress bar
							//User cancelled the operation, quit!
							EditorUtility.ClearProgressBar();			//Remove the progress bar
							AssetDatabase.Refresh();					//Do a final refresh
							EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
							return;
						}
					}
				}
				//Copy back (let alpha remain if present)
				for (Y=0; Y<Tex.height; Y++) {
					for (X=0; X<Tex.width; X++) {
						DistPixels[(Y*Tex.width)+X].r=DistPixels2[(Y*Tex.width)+X].r;	//Copy blurred red
						DistPixels[(Y*Tex.width)+X].g=DistPixels2[(Y*Tex.width)+X].g;	//Copy blurred green
					}
				}
			}
		}

		//Upload to texture
		DistortionMap.SetPixels(DistPixels);			//Write distortion map
		DistortionMap.Apply();							//Apply it
						
		//Progress bar
		if (EditorUtility.DisplayCancelableProgressBar("Height Map Tool - Generation Distortion Map...","Saving asset as PNG texture",1.0)) {	//Display progress bar
			//User cancelled the operation, quit!
			EditorUtility.ClearProgressBar();			//Remove the progress bar
			AssetDatabase.Refresh();					//Do a final refresh
			EditorUtility.DisplayDialog("Height Map Tool","Operation was cancelled - Texture may have been generated","Ok");	//Display cancellation message
			return;
		}
		
		//Save as permanent png asset
		var pngData = DistortionMap.EncodeToPNG();
        if (pngData != null) {
            File.WriteAllBytes(FilePath, pngData);
        }
		EditorUtility.ClearProgressBar();			//Remove the progress bar
        AssetDatabase.Refresh();					//Update asset folder
        
        //Set import settings for the new texture
        var textureImporter:TextureImporter = AssetImporter.GetAtPath(FilePath) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Advanced;	
        if (DistortionMap.format==TextureFormat.RGB24) {
	        textureImporter.textureFormat = TextureImporterFormat.RGB24;	//No alpha, use RGB24
		} else {
			textureImporter.textureFormat = TextureImporterFormat.RGBA32;	//Has alpha, use RGBA32 (could alternatively be ARGB32)
		}
		textureImporter.maxTextureSize=Tex.width;	//Set texture size to actual size of texture
		textureImporter.grayscaleToAlpha=false;		//Don't convert alpha channel
		textureImporter.generateCubemap=TextureImporterGenerateCubemap.None;	//Don't make a cubemap
		textureImporter.npotScale = TextureImporterNPOTScale.ToLarger;		//Try to force power of 2 but keep detail
		textureImporter.isReadable=false;			//Don't need to read it, optional
		textureImporter.mipmapEnabled=false;		//Don't mipmap by default, but it would work - optional
		textureImporter.convertToNormalmap=false;	//Don't convert to a normal map
		textureImporter.normalmap=false;			//Not a normal map
		textureImporter.lightmap=false;				//Not a lightmap
		textureImporter.anisoLevel=0;				//Don't need anisotropic to look good, optional
		textureImporter.filterMode=FilterMode.Bilinear;	//Bilinear will give good smoothness in 2D, optional trilinear in 3D
		if (target.SampleWrapX==true || target.SampleWrapY==true || target.XSmoothingWrap==true || target.YSmoothingWrap==true) {
			textureImporter.wrapMode=TextureWrapMode.Repeat;
		} else {
			textureImporter.wrapMode=TextureWrapMode.Clamp;
		}		
		//var set:TextureImporterSettings = new TextureImporterSettings();	//Update the settings
		//textureImporter.ReadTextureSettings(set);
		//st.wrapMode = TextureWrapMode.Clamp;
		//textureImporter.SetTextureSettings(set);
        AssetDatabase.ImportAsset(FilePath);		//Reimport it with new settings
	}

	function LineIntersect(CenterX:float,CenterY:float,EndX:float,EndY:float,PlaneX:float,PlaneY:float,PlaneX2:float) {
		//Find where a ray from the pixel surface to a horizontal plane intersects it, ie refracts/reflects to based on surface angle
		//CenterX,CenterY are position of pixel surface in 2D, assuming a -128 to +127 coordinate range
		//EndX,EndY are end-point of line extrapolated from CenterX,CenterY by a given angle based on the surface slope
		//PlaneX,PlaneY define the left end of a horizontal plane with which to collide, PlaneX2 is the right end of the plane
		//PlaneDistance is the Y distance of the plane from the surface pixel ie how far away is the surface being refracted
		//Assuming we're projecting from 0,0 upwards into positive pixel coordinates
		
		//Make sure we will get a collision
		if (EndY>PlaneY) {
			//Line ends outside the bounds of the plane, we can't find a collision point so we'll return the maximum distance
			if (CenterX-EndX>0) {
				//Toward the left
				return -1;
			} else {
				//Toward the right
				return 1;
			}
		}

		//Calculate where projected line from surface collides with horizontal plane
		var CollideX:float=CenterX+(EndX-CenterX) * (PlaneY-CenterY)/(EndY-CenterY);
		
		//Make sure we got a collision
		if (CollideX<PlaneX || CollideX>PlaneX2) {
			//Line collided outside the plane
			if (CenterX-EndX>0) {
				//Toward the left
				return -1;
			} else {
				//Toward the right
				return 1;
			}
		}
		
		//Return position
		if (CollideX<0) {
			//Toward the left
			return CollideX;//-((1.0/Mathf.Abs(256.0)) *Mathf.Abs(CollideX));		//Position on left side of plane
		} else {
			return CollideX;//((1.0/256.0)*CollideX);		//Position on right side of plane
		}
	}
}