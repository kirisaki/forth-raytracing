include ./pnm.fs
include ./vector.fs

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

1e 2e 3e vec3-alloc a
10e 20e 30e vec3-alloc b
create c  vec3% allot

a b c v+    \ c = a + b

\ 確認
a .v cr
b .v cr
c .v cr

a b c v-    \ c = a - b

a .v cr
b .v cr
c .v cr

a 2e c vmul  \ c = a * 2

a .v cr
c .v cr

a 2e c vdiv  \ c = a / 2

a .v cr
c .v cr

a b vdot f. cr

a b c vcross
c .v cr

a vlength f. cr

a vlength2 f. cr

a c vunit
c .v cr

." done." cr
bye