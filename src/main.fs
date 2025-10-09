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
include ./camera.fs
include ./material.fs

variable rng

\ Convert ray into color vector
: ray-color ( ray-addr depth head out-addr vp hrp sp rp -- )
  locals| rp sp hrp vp col head dep ray |
  hrp hit-record-empty locals| rec |
  dep 0<= if
    0e 0e 0e col v!
    rec hrp pool-free
    exit
  then
  0.001e inf
  ray head rec hrp vp hit if
    rp pool-alloc locals| ray-out |
    vp pool-alloc locals| att-out |
    rec h-material @ ray rec ray-out att-out vp rng @ scatter rng ! if
      ray-out dep 1- head col vp hrp sp rp recurse
      col att-out vhprod=
    else
      0e 0e 0e col v!
    then
    att-out vp pool-free
    ray-out rp pool-free
  else
    \ Background
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
: pixel-color ( color samples -- u u u )
  locals| samples color |
  1.0e samples s>f f/
  fdup color vx f@ f* fsqrt 0e 0.999e fclamp 256e f* floor f>s locals| r |
  fdup color vy f@ f* fsqrt 0e 0.999e fclamp 256e f* floor f>s locals| g |
  color vz f@ f* fsqrt 0e 0.999e fclamp 256e f* floor f>s locals| b |
  r g b
;

: generate-pnm ( width height -- width height c-addr )
  utime drop rng !
  locals| h w |
  w h * 3 * allocate throw locals| data |

  500 1024 * arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  arena ray-pool-create locals| rp |
  arena sphere-pool-create locals| sp |
  arena hit-record-pool-create locals| hrp |
  arena camera-pool-create locals| cp |
  arena material-pool-create locals| mp |

  0 locals| head |

  \ ground
  0e -1000e 0e vp vec3-new 1000e lambertian 0.5e 0.5e 0.5e vp vec3-new 0e 0e mp material-new sp sphere-new head sp push-front to head

  \ three spheres
  0e 1e 0e vp vec3-new 1e dielectric 0e 0e 0e vp vec3-new 0e 1.5e mp material-new sp sphere-new head sp push-front to head
  -4e 1e 0e vp vec3-new 1e lambertian 0.4e 0.2e 0.1e vp vec3-new 0e 0e mp material-new sp sphere-new head sp push-front to head
  4e 1e 0e vp vec3-new 1e metal 0.7e 0.6e 0.5e vp vec3-new 0.1e 0e mp material-new sp sphere-new head sp push-front to head

  \ random small spheres
  11 -11 do
    11 -11 do
      j s>f 0.9e rng @ frand rng ! f* f+
      0.2e
      i s>f 0.9e rng @ frand rng ! f* f+
      vp vec3-new locals| center |
      vp vec3-zero locals| albedo |
      vp vec3-zero vp vec3-zero locals| r1 r2 |
      rng @ r1 vrand r2 vrand rng !
      r1 r2 albedo vhprod
      r1 vp pool-free r2 vp pool-free

      vp vec3-zero locals| tmp |
      center 4e 0.2e 0e vp vec3-new tmp v- tmp vlength 0.9e f> if
        rng @ frand rng ! fdup 0.8e f< if
          \ diffuse
          center 0.2e lambertian albedo 0e 0e mp material-new sp sphere-new head sp push-front to head
          fdrop
        else 0.95e f< if
          \ metal
          center 0.2e metal albedo rng @ frand rng ! 0e mp material-new sp sphere-new head sp push-front to head
        else 
          \ glass
          center 0.2e dielectric 1e 1e 1e vp vec3-new 0e 1.5e mp material-new sp sphere-new head sp push-front to head
        then then
      then
    loop
  loop

  13e 2e 3e vp vec3-new locals| lookfrom |
  0e 0e 0e vp vec3-new locals| lookat |
  0e 1e 0e vp vec3-new locals| vup |
  pi 9e f/ \ vfov
  16e 9e f/ \ aspect ratio
  0.1e \ aperture
  10e  \ focus-dist
  lookfrom lookat vup vp cp make-camera locals| cam |
  50 locals| samples |
  20 locals| max-depth |
  vp vec3-zero locals| vzero |

  vzero vzero rp ray-new locals| ray |  
  vp vec3-zero vp vec3-zero vp vec3-zero
  locals| dir uh vv |
  0 h 1- do
    i .
    w 0 do
      arena arena-mark locals| mark |
      vp vec3-zero locals| pix |
      vp vec3-zero locals| col |
      0e 0e 0e pix v!
      samples 0 do
        j s>f rng @ frand rng ! f+ w 1- s>f f/ 
        k s>f rng @ frand rng ! f+ h 1- s>f f/ 
        cam ray rng @ vp get-ray rng !
        ray max-depth head col vp hrp sp rp ray-color
        pix col v+=
      loop

      pix samples pixel-color locals| b g r |

      h 1- j - w * i + 3 * data + 
      r over c!
      g over 1 + c!
      b over 2 + c!
      drop
      
      mark arena arena-rollback
      vp pool-reset
      rp pool-reset
      hrp pool-reset
    loop
  -1 +loop
  arena arena-destroy
  w h data
;


: main
  192 108 generate-pnm s" out.ppm" write-pnm
  \ 384 216 generate-pnm s" out.ppm" write-pnm
  \ 640 320 generate-pnm s" out.ppm" write-pnm
  \ test-vector
  \ test-list
  \ test-random
  \ test-clamp
  \ test-arena
  \ test-pool
  \ test-ray
  \ test-sphere
  \ test-hit
  \ test-camera
  \ test-material
;


main
." done." cr
bye