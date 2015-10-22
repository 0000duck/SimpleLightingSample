#version 430 core

layout (binding = 0) uniform sampler2D tlights;

layout (location = 0) in vec2 f_coord;

out vec4 color;

void main()
{
    color = vec4(texture(tlights, f_coord).xyz, 1.0);
}
