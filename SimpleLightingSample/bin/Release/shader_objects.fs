#version 430 core

layout (location = 0) in vec4 f_color;

out vec4 color;

void main()
{
    color = f_color;
}
