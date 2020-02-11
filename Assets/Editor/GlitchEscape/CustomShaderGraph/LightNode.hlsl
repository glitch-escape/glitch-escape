#ifdef LIGHTWEIGHT_LIGHTING_INCLUDED
   
   //Actual light data from the pipeline
   Light light = GetMainLight(GetShadowCoord(GetVertexPositionInputs(ObjPos)));
   Direction = light.direction;
   Color = light.color;
   ShadowAttenuation = light.shadowAttenuation;
      
#else
   
   //Hardcoded data, used for the preview shader inside the graph
   //where light functions are not available
   Direction = float3(-0.5, 0.5, -0.5);
   Color = float3(1, 1, 1);
   ShadowAttenuation = 0.4;
      
#endif