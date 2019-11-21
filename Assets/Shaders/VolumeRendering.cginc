#ifndef __VOLUME_RENDERING_INCLUDED__
#define __VOLUME_RENDERING_INCLUDED__

#include "UnityCG.cginc"

#ifndef ITERATIONS
#define ITERATIONS 100
#endif
#define DATAMAP_RGB

half4 _Color;
float _DataMapScale;
sampler3D _Volume;
sampler2D _DataMap;
float _Plane;
float3 _PickRayPos;
float3 _PickRayDir;

struct Ray {
  float3 origin;
  float3 dir;
};

struct AABB {
  float3 min;
  float3 max;
};

bool intersect(Ray r, AABB aabb, out float t0, out float t1)
{
  float3 invR = 1.0 / r.dir;
  float3 tbot = invR * (aabb.min - r.origin);
  float3 ttop = invR * (aabb.max - r.origin);
  float3 tmin = min(ttop, tbot);
  float3 tmax = max(ttop, tbot);
  float2 t = max(tmin.xx, tmin.yz);
  t0 = max(t.x, t.y);
  t = min(tmax.xx, tmax.yz);
  t1 = min(t.x, t.y);
  return t0 <= t1;
}

float3 get_uv(float3 p) {
  return (p + 0.5);
}

float sample_volume(float3 uv, float time)
{
  float v = pow( tex3D(_Volume, uv).r , 4);
  float factor = clamp(-303 + 306 * sin(uv.z * 6 + time * 2), 0.01, 3);
  return v * factor;
  //return pow(v, 3);
}

float3 sample_gradient(float3 uv)
{
  /*float3x3 sobel = {1,2,1,2,4,2,1,2,1};
  float h = 1.0/256.0;
  float3 ret = float3(0,0,0);
  
  [unroll]
  for(int i = 0; i < 3; ++i) {
    [unroll]
    for(int j = 0; j < 3; ++j) {
      float3 offset_z = float3(i-1, j-1, 0) * h;
      float3 offset_y = float3(i-1, 0, j-1) * h;
      float3 offset_x = float3(0, i-1, j-1) * h;
    
      ret.z += sobel[i][j] * (tex3D(_Volume, uv+offset_z).r - tex3D(_Volume,uv-offset_z).r);
      ret.y += sobel[i][j] * (tex3D(_Volume, uv+offset_y).r - tex3D(_Volume,uv-offset_y).r);
      ret.x += sobel[i][j] * (tex3D(_Volume, uv+offset_x).r - tex3D(_Volume,uv-offset_x).r);
    }
  }
  
  return ret / 32;*/
  
  float3 ret = float3(0,0,0);
  float h = 1.0/256.0;
  ret.x = (tex3D(_Volume, uv+float3(h,0,0)).r - tex3D(_Volume, uv-float3(h,0,0)).r);
  ret.y = (tex3D(_Volume, uv+float3(0,h,0)).r - tex3D(_Volume, uv-float3(0,h,0)).r);
  ret.z = (tex3D(_Volume, uv+float3(0,0,h)).r - tex3D(_Volume, uv-float3(0,0,h)).r);
  return ret/2;
}

struct appdata
{
  float4 vertex : POSITION;
  float2 uv : TEXCOORD0;
};

struct v2f
{
  float4 vertex : SV_POSITION;
  float2 uv : TEXCOORD0;
  float3 world : TEXCOORD1;
  float3 local : TEXCOORD2;
};

v2f vert(appdata v)
{
  v2f o;
  o.vertex = UnityObjectToClipPos(v.vertex);
  o.uv = v.uv;
  o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
  o.local = v.vertex.xyz;
  return o;
}

fixed4 frag(v2f i) : SV_Target
{ //return float4(1,1,1,1);
  Ray ray;
  ray.origin = i.local;

  // world space direction to object space
  float3 dir = (i.world - _WorldSpaceCameraPos);
  ray.dir = normalize(mul(unity_WorldToObject, dir));

  AABB aabb;
  aabb.min = float3(-0.5, -0.5, -0.5);
  aabb.max = float3(0.5, 0.5, 0.5);

  float tnear;
  float tfar;
  intersect(ray, aabb, tnear, tfar);

  tnear = max(0.0, tnear);

  float3 start = ray.origin + ray.dir * tnear;
  float3 end = ray.origin + ray.dir * tfar;

  float4 dst = float4(0, 0, 0, 0);

#ifndef DATAMAP_RGB
  [unroll]
#else
  //[unroll]
#endif
  for (int iter = 0; iter < ITERATIONS; iter++)
  {
	float iter_point = float(iter) / float(ITERATIONS);
    float3 uv = get_uv(lerp(start, end, iter_point));

    #ifdef DATAMAP_RGB
    float f = tex3D(_Volume, uv).r;
    float3 grad = sample_gradient(uv);
    float grad_mag = length(grad);
    
    float4 c = tex2D(_DataMap, float2(f, grad_mag)).rgba * _DataMapScale * 1.2;
	c += _Color * (step(_Plane-0.04, uv.z) - step(_Plane, uv.z)) * _DataMapScale * 3; // picked plane
	c += _Color * step(length(cross(uv - _PickRayPos.xyz, _PickRayDir.xyz)), 0.02) * (sin(iter_point*2 + _Time * 60)/3 + 0.5); // picking ray

    float alpha_here = dot(float3(1,1,1), c.rgb);

    dst.rgb += (1 - dst.a) * c.rgb;
    dst.a += (1 - dst.a) * alpha_here;
    #else
    
    float v = sample_volume(uv, _Time * 30);
    
    dst.rgb = ((1 - dst.a) * v * _Color) + dst.rgb;
    dst.a = (1 - dst.a) * v + dst.a;
    #endif
    
    /*float4 src = float4(v, v, v, v);
    src.a *= 0.5;
    src.rgb *= src.a;

    dst = (1.0 - dst.a) * src + dst;*/
    if (dst.a > 1) break;
  }
  return dst;
  //return saturate(dst) * _Color;
}

#endif 
