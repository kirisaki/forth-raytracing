\ Collision detection
: hit ( ray head rec -- flag ) ( tmin tmax -- )
  locals| rec head ray |
  ray
  1 cells allocate throw dup false swap !
  dup >r
  rec
  head
  [: ( ray hit-anything rec sphere -- ) ( tmin tmax -- )
    fover fover
    swap >r
    locals| sphere rec hit-anything ray |
    hit-record-empty locals| temp-rec |
    ray hit-anything rec
    sphere ray temp-rec hit-sphere if 
      true hit-anything !
      fdrop temp-rec t-val f@
      temp-rec rec hit-record% move
      temp-rec free throw
    then
    r>
  ;] foreach
  2drop drop
  fdrop fdrop 
  r> dup @ swap free throw
;
\ 1 2 3  head  [: swap >r >r dup 2over rot r> + + + . r> ;] foreach 3 0 do drop loop cr