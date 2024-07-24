#include "EntryPoint.h"
#include "Cheats/CheatFunctions.h"
#include "Game/GameFunctions.h"
#include "Game/GameFunctions.h"
#include "Utilities/Detours.h"
#include "Cheats/Calls.h"

namespace colorMgr
{
	color mw2_main;
	color glow;
	color shadow;//drop_shadow
	color popup;
	color white;
	color black;
	color stencil;
	color box;
	color gradient;
	color playercard;
	color rank;
	color hud;
	color dpad;
	color faction;
	color minimap;
	void setColor(color col)
	{
		ColorShade shade;
		mw2_main = shade.Change(col, 0);
		glow = shade.Change(col, -0.15);
		shadow = shade.Change(col, 0);
		popup = shade.Change(col, -0.15);
		white = shade.Change(col, -0.20);
		black = shade.Change(col, -0.15);
		gradient = shade.Change(col, -0.24);
		stencil = shade.Change(col, 0);
		box = shade.Change(col, -0.30);
		playercard = shade.Change(col, 0);
		rank = shade.Change(col, 0);
		hud = shade.Change(col, 0);
		dpad = shade.Change(col, 0);
		faction = shade.Change(col, 0);
		minimap = shade.Change(col, 0);
	}
};

bool colorManager(char* shader, color* col)
{
	if (strstr(shader, "mw2_main"))
	{
		*col = colorMgr::mw2_main;
		return true;
	}
	else if (strstr(shader, "glow"))
	{
		*col = colorMgr::glow;
		return true;
	}
	else if (strstr(shader, "shadow"))
	{
		*col = colorMgr::shadow;
		return true;
	}
	else if (strstr(shader, "popup"))
	{
		*col = colorMgr::popup;
		return true;
	}
	else if (strstr(shader, "white"))
	{
		*col = colorMgr::white;
		return true;
	}
	else if (strstr(shader, "black"))
	{
		*col = colorMgr::black;
		return true;
	}
	else if (strstr(shader, "stencil"))
	{
		*col = colorMgr::stencil;
		return true;
	}
	else if (strstr(shader, "box"))
	{
		*col = colorMgr::box;
		return true;
	}
	else if (strstr(shader, "gradient"))
	{
		*col = colorMgr::gradient;
		return true;
	}
	else if (strstr(shader, "playercard"))
	{
		*col = colorMgr::playercard;
		return true;
	}
	else if (strstr(shader, "rank"))
	{
		*col = colorMgr::rank;
		return true;
	}
	else if (strstr(shader, "hud"))
	{
		*col = colorMgr::hud;
		return true;
	}
	else if (strstr(shader, "dpad"))
	{
		*col = colorMgr::dpad;
		return true;
	}
	else if (strstr(shader, "faction"))
	{
		*col = colorMgr::faction;
		return true;
	}
	else if (strstr(shader, "minimap"))
	{
		*col = colorMgr::minimap;
		return true;
	}
	return false;
}

const float colorWhite[4] = { 1.0f, 1.0f, 1.0f, 1.0f };
const float colorFadedBlack[4] = { 0.0f, 0.0f, 0.0f, 0.75f };
const float color_light[4] = { 1.0f, 1.0f, 1.0f, 1.0f };

RainbowRGB fogRB;
RainbowRGB lightRB;
RainbowRGB rainbowRGB;
int rainbowSpeed = 1;
Detour<void> r_endframe;
void R_EndFrame_Detour() {

	Font_s* font = CL_RegisterFont(FONT_NORMAL, 0);
	auto material = Material_RegisterHandle(MATERIAL_WHITE, 3);

	//DrawShader(screen_res[0] / 2, (screen_res[1] / 2) - (screen_res[1] / 4), 420, 280, colorFadedBlack, MATERIAL_WHITE, ALIGN_CENTER, ALIGN_BOTTOM);
	//DrawText("", screen_res[0] / 2, (screen_res[1] / 2) - (screen_res[1] / 4) + 40, 0.95f, FONT_NORMAL, colorWhite, ALIGN_CENTER, ALIGN_TOP);
	//DrawText("", screen_res[0] / 2, (screen_res[1] / 2) - (screen_res[1] / 4) + 290, 0.55f, FONT_NORMAL, colorWhite, ALIGN_CENTER, ALIGN_BOTTOM);

	r_endframe.CallOriginal();
}

int flags = 0;

Detour<void> r_adddObjtoscene;
void R_AddDObjToScene_Detour(DObj *obj, cpose_t *pose, unsigned int entnum, unsigned int renderFxFlags, float *lightingOrigin, float materialTime) {

	//renderFxFlags |= flags;


	r_adddObjtoscene.CallOriginal(obj, pose, entnum, renderFxFlags, lightingOrigin, materialTime);
}

Detour<unsigned int> bg_getviewmodelweaponindex;
unsigned int BG_GetViewmodelWeaponIndex_Detour(playerState_s *ps) {



	return bg_getviewmodelweaponindex.CallOriginal(ps);
}


Detour<void> cl_gamepadbuttoneventforport;
void CL_GamepadButtonEventForPort_Detour(int portIndex, int key, int down, unsigned int time) {

	if (down) {
		switch (key)
		{
			case 22:
				flags--;
				printf("flags %i\n", flags);
				break;
			case 23:
				flags++;
				printf("flags %i\n", flags);
				break;
			case 21:

				break;
		}
	}

	cl_gamepadbuttoneventforport.CallOriginal(portIndex, key, down, time);
}

Detour<void> r_generatesorteddrawsurfs;
void R_GenerateSortedDrawSurfs_Detour(int viewInfoIndex, GfxSceneParms *sceneParms, GfxViewParms *viewParmsDpvs, GfxViewParms *viewParmsDraw) {
	if (tog_RGB_light)
	{
		for (int i = 0; i < 256; i++) 
		{
			sceneParms->primaryLights[i].Color = colorRGB(rainbowRGB.rgb);
		}
	}
	r_generatesorteddrawsurfs.CallOriginal(viewInfoIndex, sceneParms, viewParmsDpvs, viewParmsDraw);
}

Detour<void> r_setframefog;
void R_SetFrameFog_Detour(GfxCmdBufInput *input) {
	if (Dvar_GetBool("cl_ingame"))
	{
		if (tog_RGB_fog)
		{
			GfxBackEndData * backend = (GfxBackEndData*)(input->get<unsigned int>(0x53C));
	
			backend->fogSettings.fogStart = 50.0f;
			backend->fogSettings.sunFogScale = 8.0f;
			backend->fogSettings.color = floatColorToUInt(rainbowRGB.rgb);
		}


	}
	r_setframefog.CallOriginal(input);
}

Detour<void> r_addCmdDrawStrechPic;
void R_AddCmdDrawStretchPic_Hook(float x, float y, float w, float h, float s0, float t0, float s1, float t1, const float* col, materialx* shader)
{
	if (tog_RGB_hud)
	{
		//R_AddCmdDrawStretchPic_Detour(&x, &w, col, shader);
		const float* saveColor = col;
		color checkColor = (float*)&col;

		if (strstr(shader->material, "popup"))
		{
			col = (float*)&color(255, 0, 0, 255);//saveColor[3] * 255.0f)
		}

		if (!strcmp(shader->material, "mw2_main_cloud_overlay"))// || _sys_strcmp(shader->material, "mw2_main_cloud_overlay")
		{
			checkColor.a = 0;
			col = (float*)&checkColor;
		}
		if (!strcmp(shader->material, "mw2_main_background"))
		{
			x -= 34;
			w += (34);
		}
		if (colorManager(shader->material, &checkColor))
		{
			checkColor.a = saveColor[3];
			col = (float*)&checkColor;
		}
	}
	r_addCmdDrawStrechPic.CallOriginal(x, y, w, h, s0, t0, s1, t1, col, shader);
}

#pragma region furiousTest
namespace furious
{
	enum callBackIndex
	{
		CB_fog = 0,
		CB_light = 1,
		CB_hud = 2,
		CB_Aimbot = 3
	};
	//temp
	int memOfs = 0x82D67000;
	int memInterval = 4;
	char* getChar(int intVal)
	{
		int getOfs = memOfs + memInterval * intVal;
		return (char*)getOfs;
	}

	void setg(int intVal, int val)
	{
		int getOfs = memOfs + memInterval * intVal;
		*(int*)getOfs = val;
	}

	int g(int intVal)
	{
		int getOfs = memOfs + memInterval * intVal;
		return *(int*)getOfs;
	}

	float f(int intVal)
	{
		int getOfs = memOfs + memInterval * intVal;
		return *(float*)getOfs;
	}

	bool getBool(int intVal)
	{
		int getOfs = memOfs + memInterval * intVal;
		return *(bool*)getOfs + 3;
	}

	int rCallAddr = 0x82D67100;
	int callAddr = 0x82D67200;
	void call(int index, int value)
	{
		*(int*)(callAddr + (index * 4)) = value;
	}

	int rCall(int index)
	{
		return *(int*)(rCallAddr + (index * 4));
	}
	void rCalled(int index)
	{
		*(int*)(rCallAddr + (index * 4)) = 0;
	}

	bool callBack(int index, int *outCall)
	{
		int getCall = rCall(index);
		if (getCall > 0)
		{
			*outCall = getCall;
			rCalled(index);
			return true;
		}
		return false;
	}

	void callBackProcess()
	{
		int callFog;
		if (callBack(CB_fog, &callFog))
			tog_RGB_fog = (callFog == 2);

		int callLight;
		if (callBack(CB_light, &callLight))
			tog_RGB_light = (callLight == 2);

		int callHud;
		if (callBack(CB_hud, &callHud))
			tog_RGB_hud = (callHud == 2);

		int callAimbot;
		if (callBack(CB_Aimbot, &callAimbot))
			tog_aimbot = (callAimbot == 2);
	}

	void caller()
	{
		if (g(1) == 1)
		{
			setg(1, 0);
			call(0, 1337);
		}
	}
}

#pragma endregion

Detour<void> menuPaintAll;

void Menu_PaintAll(int a2)
{
	//drawText("testing", 100, 100, 3, FONT_NORMAL, white, align_left);
	bool inGame = Dvar_GetBool("cl_ingame");
	furious::callBackProcess();
	if (tog_RGB_fog ||tog_RGB_light || tog_RGB_hud)
		rainbowRGB.run(rainbowSpeed);

	colorMgr::setColor(color(rainbowRGB.rgb));

	if (Dvar_GetBool("cl_ingame"))
	{
		abStart = tog_aimbot;
		if (tog_aimbot)
		{
			loopFunc();
			SetAimbot(abStart);
		}
	}

	menuPaintAll.CallOriginal(a2);

}

BOOL WINAPI DllMain(HMODULE hModule, DWORD Reason, LPVOID lpVoid) {

	if (Reason == DLL_PROCESS_ATTACH) {

		screen_res = (int*)(0x83647880);
		menuPaintAll.SetupDetour(0x82285E98, Menu_PaintAll);
		//r_endframe.SetupDetour(0x82351748, R_EndFrame_Detour);
		//r_adddObjtoscene.SetupDetour(0x82352A98, R_AddDObjToScene_Detour);
		//cl_gamepadbuttoneventforport.SetupDetour(0x8213DEF8, CL_GamepadButtonEventForPort_Detour);
		r_generatesorteddrawsurfs.SetupDetour(0x823566F8, R_GenerateSortedDrawSurfs_Detour);
		r_setframefog.SetupDetour(0x82395278, R_SetFrameFog_Detour);
		r_addCmdDrawStrechPic.SetupDetour(0x821384D8, R_AddCmdDrawStretchPic_Hook);
		//bg_getviewmodelweaponindex.SetupDetour(0x820E23C8, BG_GetViewmodelWeaponIndex_Detour);
		//int weapCount = 0;
		//uint32_t* weaponComplete = (uint32_t*)(0x82557750);
		//while (weaponComplete[weapCount]) {
		//	printf("%X - %s - \"%s\"\n", weaponComplete[weapCount], *reinterpret_cast<uint32_t*>(weaponComplete[weapCount]), SEH_StringEd_GetString((const char*)(*reinterpret_cast<uint32_t*>(weaponComplete[weapCount] + 8))));
		//	weapCount++;
		//}
	}

	if (Reason == DLL_PROCESS_DETACH) {
		
		Sleep(1000);
		menuPaintAll.TakeDownDetour();
		//r_endframe.TakeDownDetour();
		//r_adddObjtoscene.TakeDownDetour();
		//cl_gamepadbuttoneventforport.TakeDownDetour();
		r_generatesorteddrawsurfs.TakeDownDetour();
		r_setframefog.TakeDownDetour();
		r_addCmdDrawStrechPic.TakeDownDetour();
		Sleep(1000);
		//bg_getviewmodelweaponindex.TakeDownDetour();
	}

	return TRUE;
}

