\ ============================
\ src/test-arena.fs  (gforth)
\ Requires:
\   align-up, arena-create, arena-alloc, arena-reset,
\   arena-mark, arena-rollback, arena-destroy
\ Optional:
\   default-align (なければ 8 を仮定)
\ ============================

[undefined] default-align [if]  8 constant default-align  [then]

: ok?    ( flag c-addr u -- )
  type s" : " type 0= if s" (test failed)" else s" (test passed)" then
  type cr
;

: =?     ( x y c-addr u -- )
  type s" : " type 2dup s" (expected) " type . s"  (actual) " type . <> if s" (test failed)" else s" (test passed)" then
  type cr
 ;

\ --- テスト用ハンドル/スロットはトップレベルで宣言 ---
65536 arena-create value TA   \ メインのテスト用アリーナ

0 value P0
0 value P1
0 value P2
0 value P3

0 value A0
0 value B0
0 value B1
0 value MARK0

0 value SMALL                 \ 小サイズアリーナ（後で to で代入）

: .case ( c-addr u -- )  ." [TEST] " type cr ;

: test-align-up ( -- )
  s" align-up basics" .case
  10 8 align-up 16 s" align-up(10,8)=16" =?
  24 8 align-up 24 s" align-up(24,8)=24" =?
  15 4 align-up 16 s" align-up(15,4)=16" =?
  true s" align-up ok" ok? ;

: test-basic-alloc ( -- )
  s" basic alloc/reset" .case
  TA arena-reset

  \ 1バイト（8境界）
  1 default-align TA arena-alloc to P0
  TA a-base @  P0  s" first alloc at base" =?
  TA a-off @   8   s" off=1 after first alloc" =?

  \ 次: 7バイト（8境界）: align-up(1,8)=8 → addr=base+8, off=15
  7 default-align TA arena-alloc to P1
  TA a-base @ 8 +  P1  s" second alloc at base+8" =?
  TA a-off @  15  s" off=15 after second alloc" =?

  \ 次: 1バイト（4境界）: align-up(15,4)=16 → addr=base+16, off=17
  1 4 TA arena-alloc to P2
  TA a-base @ 16 + P2 s" third alloc at base+16" =?
  TA a-off @  17 s" off=17 after third alloc" =?

  \ リセットで先頭に戻る
  TA arena-reset
  TA a-off @ 0 s" off=0 after reset" =?

  \ リセット後の最初の alloc は base に戻る
  8 default-align TA arena-alloc to P3
  TA a-base @ P3 s" alloc at base after reset" =?

  true s" basic alloc/reset ok" ok? ;

: test-mark-rollback ( -- )
  s" mark/rollback (LIFO)" .case
  TA arena-reset

  \ A: 32 バイト確保 → off=32
  32 default-align TA arena-alloc to A0
  TA a-off @ 32 s" off=32" =?

  \ mark
  TA arena-mark to MARK0

  \ B: 100 バイト確保（8境界）: align-up(32,8)=32 → addr=base+32, off=132
  100 default-align TA arena-alloc to B0
  TA a-base @ 32 + B0 s" B0 at base+32" =?
  TA a-off @ 132 s" off=132" =?

  \ rollback → off=mark(=32)
  MARK0 TA arena-rollback
  TA a-off @ 32 s" off back to 32" =?

  \ 再確保は同じ場所になる
  100 default-align TA arena-alloc to B1
  B1 B0 s" same address after rollback" =?

  true s" mark/rollback ok" ok? ;

: test-overflow ( -- )
  s" overflow (catch throw)" .case
  32 arena-create to SMALL
  SMALL arena-reset

  24 default-align SMALL arena-alloc drop
  [: 16 default-align SMALL arena-alloc drop ;] catch
  0<> s" overflow throws" ok?

  SMALL arena-destroy
  true s" overflow ok" ok? ;


: run-arena-tests ( -- )
  cr ." ===== Arena Tests =====" cr
  test-align-up
  test-basic-alloc
  test-mark-rollback
  test-overflow
  cr ." All arena tests passed." cr ;

\ 自動実行したいときは以下をアンコメント
run-arena-tests
