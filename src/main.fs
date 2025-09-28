include ./util.fs
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

: ray-color ( ray head depth -- color )
  locals| d head ray |
  hit-record-empty locals| rec |
  d 0<= if
    0e 0e 0e vec3-new
    exit
  then
  0.001e inf
  ray head rec hit if
    rec rec-material ray rec rng @ scatter rng ! 
    if locals| att out-ray |
      \ scattered
      out-ray head d 1- recurse att vhprod 
      out-ray free throw
    else
      \ absorb
      free throw drop 0e 0e 0e vec3-new 
    then
  else
    \ background
    ray direction vunit
    vy f@ 1e f+ 2e f/
    fdup 1e fswap f-
    1e 1e 1e vec3-new dup
    vmul swap free throw
    0.5e 0.7e 1.0e vec3-new dup
    vmul swap free throw
    over over v+ -rot free throw free throw
  then
  rec free throw
;

: pixel-color ( color samples -- u u u )
  locals| samples color |
  1.0e samples s>f f/
  fdup color vx f@ f* fsqrt 0e 0.999e fclamp 256e f* floor f>s locals| r |
  fdup color vy f@ f* fsqrt 0e 0.999e fclamp 256e f* floor f>s locals| g |
  color vz f@ f* fsqrt 0e 0.999e fclamp 256e f* floor f>s locals| b |
  r g b
;

: generate-pnm ( u-width u-height -- width-u height-u c-addr )
  utime drop rng !
  locals| h w |
  w h * 3 * allocate throw locals| addr |
  13e 2e 3e vec3-new \ lookfrom
  0e 0e 0e vec3-new \ lookat
  0e 1e 0e vec3-new \ vup
  pi 9e f/ \ vfov
  16e 9e f/ \ aspect ratio
  0.1e \ aperture
  10e \ dist to focus
  make-camera locals| cam |

  0 locals| head |
  \ ground
  0e -1000e 0e vec3-new 1000e lambertian 0.5e 0.5e 0.5e color-new 0e 0e material-new sphere-new head push-front to head

  \ three spheres
  0e 1e 0e vec3-new 1e dielectric 0e 0e 0e color-new 0e 1.5e material-new sphere-new head push-front to head
  -4e 1e 0e vec3-new 1e lambertian 0.4e 0.2e 0.1e color-new 0e 0e material-new sphere-new head push-front to head
  4e 1e 0e vec3-new 1e metal 0.7e 0.6e 0.5e color-new 0e 0e material-new sphere-new head push-front to head

  \ random small spheres
  \ 11 -11 do
  \   11 -11 do
  \     j s>f 0.9e rng @ frand rng ! f* f+
  \     0.2e
  \     i s>f 0.9e rng @ frand rng ! f* f+
  \     vec3-new locals| center |
  \     rng @ vrand swap  vrand swap rng ! over over vhprod locals| albedo |
  \     free throw free throw

  \     center 4e 0.2e 0e vec3-new v- vlength 0.9e f> if
  \       rng @ frand rng ! fdup 0.8e f< if
  \         \ diffuse
  \         center 0.2e lambertian albedo 0e 0e material-new sphere-new head push-front to head
  \         fdrop
  \       else 0.95e f< if
  \         \ metal
  \         center 0.2e metal albedo rng @ frand rng ! 0e material-new sphere-new head push-front to head
  \       else 
  \         \ glass
  \         center 0.2e dielectric 1e 1e 1e color-new 0e 1.5e material-new sphere-new head push-front to head
  \       then then
  \     then
  \   loop
  \ loop

  h 1- 0 swap do
    i .
    w 0 do
      0e 0e 0e vec3-new locals| pix |
      100 0 do
        j s>f rng @ frand rng ! f+ w 1- s>f f/
        k s>f rng @ frand rng ! f+ h 1- s>f f/
        cam rng @ get-ray rng !
        head 50 ray-color
        pix v+ to pix
      loop
      pix 100 pixel-color
      h 1- j - w * i + 3 * addr +
      dup 2 +
      rot swap c!
      dup 1 +
      rot swap c!
      c!
    loop
  -1 +loop
  w h
  addr
;

192 108 generate-pnm s" test1.pnm" write-pnm
\ 384 216 generate-pnm s" test1.pnm" write-pnm


\ test-vector
\ test-ray
\ test-sphere
\ test-list
\ test-random
\ test-clamp
\ test-material

\ 1e 2e 3e vec3-alloc orig
\ 2e 3e 4e vec3-alloc dir
\ orig dir ray-alloc r
\ create x vec3% allot
\ r ray-color
\ dup .v cr
\ pixel-color . . . cr

." done." cr
bye