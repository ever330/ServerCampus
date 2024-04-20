# Web API 관련 공부 내용 정리

* DI 생명 주기
  - AddSingle : 웹 서버 시작 시 생성되며, 웹 서버가 종료될 때 까지 유지된다.
  - AddScoped : 클라이언트 요청 당 한번만 생성된다. 동일한 클라이언트에 대하여 같은 Instance 유지.
  - AddTransient : 각 요청 마다 Instance가 생성된다.
 
* SQLKata
  1. MySQLConnection와 MySQLCompiler 클래스를 사용하여 연결할 DB를 정해준다.
  2. QueryFactory 클래스의 생성자로 MySQLConnection와 MySQLCompiler의 객체를 넣어 생성해준다.
  3. QueryFactory의 Query 메소드를 사용하여 쿼리문을 실행시킨다.

* CloudStructure
  1. RedisConfig 클래스를 사용하여 Redis 설정을 해준다.
  2. RedisConnection 클래스의 생성자에 위에서 설정한 RedisConfig 객체를 넣어 생성해준다.
  3. RedisString<>의 매소드에 Redis에 보관할 Value 형식을 넣고, 생성자에 RedisConnection과 값과 유지기간을 넣어 생성해준다.
  4. RedisString<>의 객체를 사용하여 Redis에 데이터를 읽거나 쓴다.
