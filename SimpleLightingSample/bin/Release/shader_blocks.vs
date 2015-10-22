#version 430 core

layout (location = 0) in vec2 position;

layout (location = 1) uniform mat4 projection;
layout (location = 2) uniform mat4 model_matrix;

layout (location = 0) out vec2 f_coord;

void main()
{
    // TODO: Handle f_coord less stupid. This can't be tessellated and is rather slow.
    f_coord = vec2(0.0, 0.0);
    if (position.x == 0.0) { f_coord.x = 0.0; } else { f_coord.x = 1.0; }
    if (position.y == 0.0) { f_coord.y = 0.0; } else { f_coord.y = 1.0; }
	gl_Position = projection * model_matrix * vec4(position.x, position.y, 0.0, 1.0);
}
