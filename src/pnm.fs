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