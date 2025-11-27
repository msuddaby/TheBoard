<script lang="ts" setup>
// SHUT UP!!!
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck
// https://godotshaders.com/shader/balatro-fire-shader/

import { onMounted, onUnmounted, ref, watch } from 'vue'

interface Props {
  bottomColor?: [number, number, number]
  middleColor?: [number, number, number]
  topColor?: [number, number, number]
  fireAlpha?: number
  fireSpeed?: [number, number]
  fireAperture?: number
}

const props = withDefaults(defineProps<Props>(), {
  bottomColor: () => [0.0, 0.7, 1.0],
  middleColor: () => [1.0, 0.5, 0.0],
  topColor: () => [1.0, 0.9, 0.3],
  fireAlpha: 1.0,
  fireSpeed: () => [0.0, 0.8],
  fireAperture: 0.22,
})

const canvasRef = ref<HTMLCanvasElement | null>(null)
let animationId: number
let gl: WebGLRenderingContext | null = null
let uniforms: Record<string, WebGLUniformLocation | null> = {}

const vertexShaderSource = `
  attribute vec2 a_position;
  varying vec2 v_uv;
  void main() {
    v_uv = a_position * 0.5 + 0.5;
    gl_Position = vec4(a_position, 0.0, 1.0);
  }
`

const fragmentShaderSource = `
  precision mediump float;
  varying vec2 v_uv;
  
  uniform float u_time;
  uniform float u_aspect;
  uniform vec3 u_bottom_color;
  uniform vec3 u_middle_color;
  uniform vec3 u_top_color;
  uniform float u_fire_alpha;
  uniform vec2 u_fire_speed;
  uniform float u_fire_aperture;
  
  float hash(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453);
  }
  
  float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    
    float a = hash(i);
    float b = hash(i + vec2(1.0, 0.0));
    float c = hash(i + vec2(0.0, 1.0));
    float d = hash(i + vec2(1.0, 1.0));
    
    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
  }
  
  float fbm(vec2 p) {
    float value = 0.0;
    float amplitude = 0.5;
    for (int i = 0; i < 6; i++) {
      value += amplitude * noise(p);
      p *= 2.2;
      amplitude *= 0.45;
    }
    return value;
  }
  
vec3 tri_color_mix(vec3 color1, vec3 color2, vec3 color3, float pos) {
  pos = clamp(pos, 0.0, 1.0);
  
  // Sharper transitions using smoothstep
  float t1 = smoothstep(0.0, 0.4, pos);  // bottom to middle
  float t2 = smoothstep(0.4, 0.7, pos);  // middle to top
  
  vec3 result = mix(color1, color2, t1);
  return mix(result, color3, t2);
}
  
void main() {
  vec2 uv = v_uv;
  uv.y = 1.0 - uv.y;
  
  // Create flame tongues - wave pattern along X
  float tongue_freq = 14.0;  // Number of flame peaks
  float tongue_wave = sin(uv.x * tongue_freq * 3.14159) * 0.5 + 0.5;
  tongue_wave = pow(tongue_wave, 1.9);  // Sharpen the peaks
  
  // Animate the tongues slightly
  float tongue_shift = sin(u_time * 2.0 + uv.x * 10.0) * 0.1;
  tongue_wave += tongue_shift;
  
  vec2 scaled_uv = vec2(uv.x * u_aspect, uv.y);
  
  vec2 shifted_uv1 = scaled_uv * 8.0 + u_time * u_fire_speed;
  vec2 shifted_uv2 = scaled_uv * 3.0 + u_time * u_fire_speed * 1.5;
  
  float fire_noise1 = fbm(shifted_uv1);
  float fire_noise2 = fbm(shifted_uv2);
  
  float combined_noise = (fire_noise1 + fire_noise2) * 0.5;
  
  float fire_noise = uv.y * (((uv.y + u_fire_aperture) * combined_noise - u_fire_aperture) * 120.0);
  fire_noise += sin(uv.y * 10.0 + u_time * 2.0) * 0.1;
  
  // Apply tongue modulation - reduces fire between peaks
  float tongue_strength = 0.4;  // How much separation (0 = none, 1 = full)
  fire_noise *= mix(1.0, tongue_wave, tongue_strength * (1.0 - uv.y * 0.5));
  
  float gradient_pos = clamp(fire_noise * 0.08, 0.0, 1.0);
  float height_bias = smoothstep(0.3, 0.8, uv.y);
  gradient_pos = mix(gradient_pos, 1.0, height_bias * 0.5);
  vec3 fire_color = tri_color_mix(u_bottom_color, u_middle_color, u_top_color, gradient_pos);
  
  float alpha = clamp(fire_noise, 0.0, 1.0) * u_fire_alpha;
  
  gl_FragColor = vec4(fire_color, alpha);
}
`

function createShader(gl: WebGLRenderingContext, type: number, source: string): WebGLShader | null {
  const shader = gl.createShader(type)
  if (!shader) return null
  gl.shaderSource(shader, source)
  gl.compileShader(shader)
  if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
    console.error('Shader compile error:', gl.getShaderInfoLog(shader))
    gl.deleteShader(shader)
    return null
  }
  return shader
}

function updateUniforms() {
  if (!gl) return
  gl.uniform3f(uniforms.bottomColor, ...props.bottomColor)
  gl.uniform3f(uniforms.middleColor, ...props.middleColor)
  gl.uniform3f(uniforms.topColor, ...props.topColor)
  gl.uniform1f(uniforms.fireAlpha, props.fireAlpha)
  gl.uniform2f(uniforms.fireSpeed, ...props.fireSpeed)
  gl.uniform1f(uniforms.fireAperture, props.fireAperture)
}

function handleResize() {
  const canvas = canvasRef.value
  if (!canvas || !gl) return

  const rect = canvas.getBoundingClientRect()
  const dpr = window.devicePixelRatio || 1

  canvas.width = rect.width * dpr
  canvas.height = rect.height * dpr

  gl.viewport(0, 0, canvas.width, canvas.height)
  gl.uniform1f(uniforms.aspect, rect.width / rect.height)
}

function initWebGL(canvas: HTMLCanvasElement) {
  gl = canvas.getContext('webgl', { alpha: true, premultipliedAlpha: false })
  if (!gl) {
    console.error('WebGL not supported')
    return false
  }

  const vertexShader = createShader(gl, gl.VERTEX_SHADER, vertexShaderSource)
  const fragmentShader = createShader(gl, gl.FRAGMENT_SHADER, fragmentShaderSource)
  if (!vertexShader || !fragmentShader) return false

  const program = gl.createProgram()
  if (!program) return false
  gl.attachShader(program, vertexShader)
  gl.attachShader(program, fragmentShader)
  gl.linkProgram(program)

  if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
    console.error('Program link error:', gl.getProgramInfoLog(program))
    return false
  }

  const positions = new Float32Array([-1, -1, 1, -1, -1, 1, 1, 1])
  const buffer = gl.createBuffer()
  gl.bindBuffer(gl.ARRAY_BUFFER, buffer)
  gl.bufferData(gl.ARRAY_BUFFER, positions, gl.STATIC_DRAW)

  const positionLoc = gl.getAttribLocation(program, 'a_position')
  gl.enableVertexAttribArray(positionLoc)
  gl.vertexAttribPointer(positionLoc, 2, gl.FLOAT, false, 0, 0)

  gl.useProgram(program)

  uniforms = {
    time: gl.getUniformLocation(program, 'u_time'),
    aspect: gl.getUniformLocation(program, 'u_aspect'),
    bottomColor: gl.getUniformLocation(program, 'u_bottom_color'),
    middleColor: gl.getUniformLocation(program, 'u_middle_color'),
    topColor: gl.getUniformLocation(program, 'u_top_color'),
    fireAlpha: gl.getUniformLocation(program, 'u_fire_alpha'),
    fireSpeed: gl.getUniformLocation(program, 'u_fire_speed'),
    fireAperture: gl.getUniformLocation(program, 'u_fire_aperture'),
  }

  gl.enable(gl.BLEND)
  gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA)

  updateUniforms()
  handleResize()

  return true
}

watch(() => props, updateUniforms, { deep: true })

onMounted(() => {
  const canvas = canvasRef.value
  if (!canvas) return

  if (!initWebGL(canvas)) return

  window.addEventListener('resize', handleResize)

  const resizeObserver = new ResizeObserver(handleResize)
  resizeObserver.observe(canvas)

  const startTime = performance.now()

  function render() {
    if (!gl || !uniforms.time) return

    const elapsed = (performance.now() - startTime) / 1000
    gl.uniform1f(uniforms.time, elapsed)

    gl.clearColor(0, 0, 0, 0)
    gl.clear(gl.COLOR_BUFFER_BIT)
    gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4)

    animationId = requestAnimationFrame(render)
  }

  render()

  onUnmounted(() => {
    cancelAnimationFrame(animationId)
    window.removeEventListener('resize', handleResize)
    resizeObserver.disconnect()
  })
})
</script>

<template>
  <canvas ref="canvasRef" class="flame-canvas" />
</template>

<style scoped>
.flame-canvas {
  width: 100%;
  height: 100%;
  pointer-events: none;
}
</style>
