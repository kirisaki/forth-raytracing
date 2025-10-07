include ./util.fs
include ./arena.fs
include ./pool.fs
include ./pnm.fs
include ./vector.fs
include ./ray.fs
include ./sphere.fs
include ./list.fs
include ./hit.fs
include ./random.fs

variable rng

\ Convert ray into color vector
: ray-color ( ray-addr head out-addr vp hrp sp -- )
  locals| sp hrp vp col head ray |
  hrp hit-record-empty locals| rec |
  0e inf
  ray head rec hrp vp hit if
    1e 1e 1e col v!
    col rec h-normal v+=
    col 2e vdiv=
  else
    vp vec3-zero locals| unit-dir |
    ray r-direction unit-dir vunit
    unit-dir vy f@ 1.0e f+ 2.0e f/
    fdup 1.0e fswap f- 

    1e 1e 1e vp vec3-new locals| white |
    white vmul=
    0.5e 0.7e 1.0e vp vec3-new locals| blue |
    blue vmul=
    white blue v+=
    white col vec3-move

    blue vp pool-free
    white vp pool-free
    unit-dir vp pool-free
  then
  rec hrp pool-free
;

\ Color to a pixel
: pixel-color ( color -- u u u )
  locals| color |
  color vx f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vy f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vz f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
;
: generate-pnm ( width height -- width height c-addr )
  locals| h w |
  w h * 3 * allocate throw locals| data |

  50 1024 * arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  arena ray-pool-create locals| rp |
  arena sphere-pool-create locals| sp |
  arena hit-record-pool-create locals| hrp |

  0 locals| head |
  0e 0e -1e vp vec3-new 0.5e sp sphere-new head sp push-front to head
  0e -100.5e -1e vp vec3-new 100e sp sphere-new head sp push-front to head

  3.555555e 0e 0e vp vec3-new locals| horizontal | \ 3.555... = viewport height(2.0) * aspect ratio(16/9)
  0e 2e 0e vp vec3-new locals| vertical |
  0e 0e 0e vp vec3-new locals| orig |
  0e 0e 1e vp vec3-new locals| focal |
  vp vec3-zero vp vec3-zero locals| h/2 v/2 |
  horizontal h/2 2e vdiv
  vertical v/2 2e vdiv
  vp vec3-zero locals| llc |
  orig llc vec3-move
  llc h/2 v-=
  llc v/2 v-=
  llc focal v-=

  arena arena-mark locals| mark |
  orig orig rp ray-new locals| ray |  
  vp vec3-zero vp vec3-zero vp vec3-zero vp vec3-zero 
  locals| dir uh vv col |
  0 h 1- do
    w 0 do
      i s>f w 1- s>f f/ 
      j s>f h 1- s>f f/ 

      vertical vv vmul
      horizontal uh vmul
      llc dir vec3-move
      dir uh v+=
      dir vv v+=
      dir orig v-=
      dir ray r-direction vec3-move
      ray head col vp hrp sp ray-color
      col pixel-color locals| b g r |

      h 1- j - w * i + 3 * data + 
      r over c!
      g over 1 + c!
      b over 2 + c!
      drop
      
      mark arena arena-rollback
    loop
  -1 +loop
  arena arena-destroy
  w h data
;


: main
  384 216 generate-pnm s" out.ppm" write-pnm
  \ test-vector
  \ test-list
  \ test-random
  \ test-clamp
  \ test-arena
  \ test-pool
  \ test-ray
  \ test-sphere
  \ test-hit
;


main
." done." cr
bye