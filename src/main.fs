include ./util.fs
include ./arena.fs
include ./pool.fs
include ./pnm.fs
include ./vector.fs
include ./ray.fs
include ./list.fs
include ./random.fs

variable rng

: generate-pnm ( width height -- width height c-addr )
  locals| h w |
  w h * 3 * allocate throw locals| data |

  30 1024 * arena-create locals| arena |
  arena vec3-pool-create locals| vp |
  arena ray-pool-create locals| rp |
  
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
      ray col vp ray-color
      col pixel-color locals| b g r |
      \ r . g . b . s"  - " type

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
;


main
." done." cr
bye