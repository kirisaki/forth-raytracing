include ./util.fs
include ./pnm.fs
include ./vector.fs
include ./list.fs
include ./random.fs

variable rng

: generate-pnm ( width height -- width height c-addr )
  locals| h w |
  w h * 3 * allocate throw locals| data |
  
  0 h 1- do
    w 0 do
      i s>f w 1- s>f f/ 255.999e f* f>s locals| r |
      j s>f h 1- s>f f/ 255.999e f* f>s locals| g |
      0.25e 255.999e f* f>s locals| b |

      h 1- j - w * i + 3 * data + 
      r over c!
      g over 1 + c!
      b over 2 + c!
      drop
    loop
  -1 +loop
  w h data
;


: main
  240 135 generate-pnm s" out.ppm" write-pnm
  \ test-vector
  \ test-list
  \ test-random
  \ test-clamp
;

main
." done." cr
bye