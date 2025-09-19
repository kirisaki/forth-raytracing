: w8   ( c fd -- )  >r pad c!  pad 1 r> write-file throw ;

: wstr ( c-addr u fd -- )  write-file throw ;

: u>str ( u -- c-addr u )
  0 <# #s #> ;

: write-pnm ( width-u height-u c-addr filename-addr filename-u -- c-addr )
  ." Writing data to " 2dup type cr
  2dup delete-file drop
  w/o create-file throw >r
  -rot 2dup swap
  s" P6" r@ wstr
  10 r@ w8
  u>str r@ wstr 32 r@ w8
  u>str r@ wstr 10 r@ w8
  s" 255" r@ wstr 10 r@ w8
  * 3 * r@ write-file throw
  r> close-file throw
;

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

256 256 generate-pnm
s" test1.pnm" write-pnm

." done." cr
bye