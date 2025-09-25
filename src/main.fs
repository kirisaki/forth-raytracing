include ./pnm.fs
include ./vector.fs
include ./ray.fs
include ./sphere.fs
include ./list.fs
include ./hit.fs
include ./camera.fs
include ./random.fs
include ./util.fs

variable rng

: ray-color ( ray head depth -- color )
  locals| depth head ray |
  hit-record-empty locals| rec |
  depth 0<= if
    0e 0e 0e vec3-new
    exit
  then
  0.001e inf
  ray head rec hit if
    rec point @ dup rec normal @ v+ rec normal @ rng @ vrand-in-hemisphere swap rng ! v+
    rec point @ v- ray-new head depth 1- recurse 0.5e vmul
  else
    ray direction vunit
    vy f@ 1e f+ 2e f/
    fdup 1e fswap f-
    1e 1e 1e vec3-new
    vmul
    0.5e 0.7e 1.0e vec3-new
    vmul
    v+
  then
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
  default-camera locals| cam |
  3.555555e 0e 0e vec3-new locals| horizontal | \ 3.555... = viewport height(2.0) * aspect ratio(16/9)
  0e 2e 0e vec3-new locals| vertical |
  0e 0e 0e vec3-new locals| orig |
  horizontal 2e vdiv locals| llc-h |
  vertical 2e vdiv locals| llc-v |
  0e 0e 1e vec3-new locals| llc-f |
  orig llc-h v- llc-v v- llc-f v- locals| lower-left-corner |

  0 locals| head |
  0e 0e -1e vec3-new 0.5e sphere-new head push-front to head
  0e -100.5e -1e vec3-new 100e sphere-new head push-front to head

  h 1- 0 swap do
    i .
    w 0 do
      0e 0e 0e vec3-new locals| pix |
      100 0 do
        j s>f rng @ frand rng ! f+ w 1- s>f f/
        k s>f rng @ frand rng ! f+ h 1- s>f f/
        cam get-ray
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

384 216 generate-pnm s" test1.pnm" write-pnm


\ test-vector
\ test-ray
\ test-sphere
\ test-list
\ test-random
\ test-clamp

\ 1e 2e 3e vec3-alloc orig
\ 2e 3e 4e vec3-alloc dir
\ orig dir ray-alloc r
\ create x vec3% allot
\ r ray-color
\ dup .v cr
\ pixel-color . . . cr

." done." cr
bye