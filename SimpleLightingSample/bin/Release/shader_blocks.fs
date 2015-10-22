#version 430 core

layout (binding = 0) uniform sampler2D blocks;

layout (location = 0) in vec2 f_coord;

out vec4 color;

vec4 pickColor(float myr)
{
    // TODO: Should this be done CPU-side?
    switch (int((myr * 256)))
    {
        // TODO: Why must these be doubled?
        case 0:
            return vec4(0.0, 0.0, 0.0, 0.0);
        case 2:
            return vec4(0.5, 0.5, 0.5, 1.0);
        case 4:
            return vec4(0.0, 1.0, 1.0, 1.0);
        default:
            return vec4(0.0, 0.0, 0.0, 0.0);
    }
}

void main()
{
    color = pickColor(texture(blocks, f_coord).r);
}
