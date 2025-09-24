include ./pnm.fs
include ./vector.fs
include ./ray.fs
include ./sphere.fs
include ./list.fs
include ./hit.fs
include ./random.fs
include ./util.fs

: ray-color ( ray head -- color )
  locals| head ray |
  hit-record-empty locals| rec |
  0e inf
  ray head rec hit if
    rec normal @ 1e 1e 1e vec3-new v+ 2e vdiv
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

: pixel-color ( color -- u u u )
  locals| color |
  color vx f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vy f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
  color vz f@ 255.999e f* f>s dup 0< if drop 0 then dup 255 > if drop 255 then
;

: generate-pnm ( u-width u-height -- width-u height-u c-addr )
  locals| h w |
  w h * 3 * allocate throw locals| addr |
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
    w 0 do
      orig
      lower-left-corner
      horizontal i s>f w 1- s>f f/ vmul v+
      vertical j s>f h 1- s>f f/ vmul v+
      orig v-
      ray-new head ray-color pixel-color
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

\ 384 216 generate-pnm
\ 4 3 generate-pnm
\ s" test1.pnm" write-pnm

\ test-vector
\ test-ray
\ test-sphere
\ test-list
\ test-random
test-clamp

\ 1e 2e 3e vec3-alloc orig
\ 2e 3e 4e vec3-alloc dir
\ orig dir ray-alloc r
\ create x vec3% allot
\ r ray-color
\ dup .v cr
\ pixel-color . . . cr

." done." cr
bye