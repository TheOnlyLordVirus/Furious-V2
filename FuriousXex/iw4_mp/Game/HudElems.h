#pragma once

#include <stdio.h>
#include <math.h>
typedef struct materialx
{
	char* material;
};

struct Shader_s
{
	materialx* material;
} Shader;
