include ./pnm.fs
include ./vector.fs
include ./ray.fs

: generate-pnm ( u-width u-height -- width-u height-u c-addr )
  dup locals| h |
  over locals| w |
  2dup * 3 * dup cells allocate throw locals| addr |
  drop
  0 do
  dup
    0 do
      w j * i + 3 * addr +
      dup
      i s>f w s>f f/ 255.999e f* f>s swap c!
      dup 1 +
      h j - s>f h s>f f/ 255.999e f* f>s swap c!
      2 +
      0.25e 255.999e f* f>s swap c!
    loop
  loop
  drop w h
  addr
  .s cr
;

\ 256 256 generate-pnm
\ s" test1.pnm" write-pnm

test-vector
test-ray

." done." cr
bye