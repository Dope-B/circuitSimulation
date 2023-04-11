# circuitSimulation

#### 사용 기술: Unity(2020.3.32f1)
#### 제작 기간: 2021.11.09~2021.01.25
#### 설명
- 마우스 휠로 카메라 확대 및 축소가 가능하다.(MouseControl에 구현)
  - 마우스 휠 조작 시 마우스가 매뉴텝에 있다면 줌을 안하고 매뉴탭을 스크롤링 한다.
```C#
// CamMove
public void zoomIn() { cam.orthographicSize = Mathf.Max(maxZoom, cam.orthographicSize - zSpeed);limitCamBound(); }
public void zoomOut(){ cam.orthographicSize = Mathf.Min(minZoom, cam.orthographicSize + zSpeed); limitCamBound(); }
.
.
.// MouseControl
float scroll = Input.GetAxis("Mouse ScrollWheel");
if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) { CamMove.cam.GetComponent<CamMove>().zoomIn(); }
else
  {
      hit = Physics2D.Raycast(CamMove.cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f, 1 << 5);
      if (hit) {
           switch (hit.collider.gameObject.tag)// 마우스가 매뉴탭에 있을 시 매뉴탭 스크롤링
              {
                  case "menu":
                        menuScroll.GetComponent<Scrollbar>().value = Mathf.Min(1, menuScroll.GetComponent<Scrollbar>().value + 0.1f);
                        break;
                   default: break;
              }
      }
      else { }
  }
```
 - 마우스 우클릭 중 드래그로 화면 이동이 가능하다.
 
 ```C#
     public void camDrag(bool b1)
    {
        if (b1)// b1->Input.GetMouseButton(1)
        {
            CMgap = cam.transform.position - cam.ScreenToWorldPoint(Input.mousePosition);// 카메라와 마우스위치 차이
            if (!isCamClicked)
            {
                originPos = cam.ScreenToWorldPoint(Input.mousePosition);// 최초 마우스 위치 저장
                isCamClicked = true;
            }
        }
        else { isCamClicked = false; }
        if (isCamClicked)
        {
            cam.transform.position = originPos + CMgap;// 마우스를 움직인 만큼 위치 재설정
            float x1 = Mathf.Clamp(originPos.x + CMgap.x,// 카메라 위치 제한
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.min.x + halfWidth,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.max.x - halfWidth);
            float y1 = Mathf.Clamp(originPos.y + CMgap.y,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.min.y + halfHeight,
                                   grid.gameObject.GetComponent<BoxCollider2D>().bounds.max.x - halfHeight);
            cam.transform.position = new Vector3(x1, y1, (originPos.z + CMgap.z));
        }
    }
 ```
 - 마우스 좌클릭으로 소자를 배치할 수 있다.(MouseControl-> drawWire(),dragComponents())
   - 기존 배치된 소자 좌클릭 시 새로운 소자를 배치하지 않고 클릭 소자를 포커스하고 포커스중인 소자는 드래그로 위치 이동이 가능하다.
     - 드래그된 위치에 다른 소자가 있다면 포커스된 소자를 삭제한다.
   - 포커스된 소자에 대한 정보는 하단 정보탭에 출력되고 수정 가능하다.
   - 소자 배치 중 다른 소자와 위치가 중복될 경우 소자를 배치하지 않는다.
   - 전선 포커스 시 전선의 형태변형이 가능하다.
     - 전선형태 변형 시 그 크기에 맞는 콜라이더가 생성된다.
     ```C#
         void getColPoint(Vector2 v1)// v1은 전선(직선)의 법선벡터
        {
            if (v1.x == 0)// a1-> 기울기  x-> 콜라이더 점들의 x좌표 변화  y-> 콜라이더 점들의 y좌표 변화
              {
                  a1 = 0;
                 x = 0;
                 y = (width * 0.6f);// width-> 콜라이더 두께
              }
            else
              {
                  a1 = v1.y / v1.x;
                  x = (width * 0.6f) / Mathf.Sqrt(Mathf.Pow(a1, 2) + 1);
                  y = a1 * x;
              }
        }
     
     ```
 - 포커스된 소자들은 투명도가 낮아진다.
 ```C#
     public virtual void setFocused(bool a)
    {
        isFocused = a;
        if (isFocused) { this.gameObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.5f); }
        else { this.gameObject.GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.5f); }
    }
 ```
 - 소자 배치 화면은 격자무늬로 되어있으며 이는 쉐이더로 구현하였다.
 - 모든 소자는 격자무늬 교차점에 마그네틱 효과를 받는다.(격자무늬 간격은 1이다.)
 ```C#
     bool magnetic(float min, float max, Vector2 gap)// min(0~1f)-> 범위 하한선 max(0~1f)-> 범위 상한선 gap-> 보정치(마우스 클릭 위치와 소자 위치간 차)
    {
        if ((Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x - gap.x) % 1) <= min ||// 마우스 위치(소수점만 받음)가 min~max면 true return
        Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).x - gap.x) % 1) >= max)
        && 
        (Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y - gap.y) % 1) <= min ||
        Mathf.Abs((CamMove.cam.ScreenToWorldPoint(Input.mousePosition).y - gap.y) % 1) >= max))
        { return true; }
        else { return false; }
    }
 
 //예시
 if (magnetic(0.3f, 0.7f, Vector2.zero)){// 실행코드}// 교차점 기준 30%까지 자석 효과
 ```
 
 - 소자 드래그 중 마그네틱 효과를 안 받는 상태에서 드래그를 중지했을 시 소자를 삭제한다.
   - 전선의 경우 드래그중인 점을 삭제하고 점의 갯수가 2개 미만이라면 전선을 삭제한다.
 - 키보드 입력으로도 소자배치가 가능하고 정보탭도 열 수 있다.(KeyControl 구현)
 - 정보탭에서 소자를 90도 단위로 회전시킬 수 있다.
   - 소자 회전 중 다른 소자와 위치가 겹치게 된다면 다시 회전한다.
 - 모든 소자는 Components를 상속받고 속성은 다음과 같다.
   - 연결점의 좌표
   - 연결된 다른 소자
   - 각 연결점 정보
     - 좌표
     - 연결된 소자
     - 연결된 노드
   - 포커스 여부
 - 회로 해석은 KCL을 사용한다.(ComponentManager에 루프구성 구현)
   - 배치된 소자들의 연결점을 확인 후 연결점이 중복된다면 이어져있는 것으로 간주한다.
   - 연결된 소자들에 대한 정보는 앞서 적혀있는 연결점 정보에 추가한다.
   - KCL해석을 위해 전선을 제외한 소자마다 노드를 할당한다.
   - 각 노드에 걸려야할 전압을 행렬로 만든 후 연결된 소자들에 따라 방정식을 만든다.
   - 방정식을 풀기위해 역행렬을 이용하여 푼다.(CircuitAnalyzer)
      ```C#
      List<float> voltage = new List<float>();
      voltage.AddRange(getData(matrix, matrix2, getDet(matrix)));// getDet-> 행렬의 크기를 구함 getData-> 방정식 해석
      .
      .
      .
       float getDet(List<float[]> matrix)
        {
            float det = 0;
            if (matrix.Count > 2)// 크기가 3x3 매트릭스 이상일 시 
            {
                float[] box = new float[matrix[0].Length];// 첫 행과 첫 열을 제외한 행렬의 크기를 함구함
                for (int i = 0; i < matrix[0].Length; i++)
                {
                    // 첫 행 제거
                    // i 열 제거
                    if (matBox[0].Length > 2) { box[i] = getDet(matBox); }// 삭제 후 행렬의 크기가 3x3 이상일 시 함수를 다시 실행
                    else { box[i] = (matBox[0][0] * matBox[1][1]) - (matBox[0][1] * matBox[1][0]); }
                }
                for (int i = 0; i < box.Length; i++)
                {
                    det += matrix[0][i] * box[i] * Mathf.Pow(-1, i);// 행렬 크기 
                }
            }
            else if (matrix.Count == 2) { det = (matrix[0][0] * matrix[1][1]) - (matrix[0][1] * matrix[1][0]); }// 매트릭스 크기가 2
            else if (matrix.Count == 1) { det = matrix[0][0]; }// 매트릭스 크기가 1
            return det;
        }
        .
        .
        .//getData
        if (det == 0) { Debug.Log("Error"); isErrorOccurred = true; return new List<float>(); }// 행렬의 크기가 0일 시 에러 
      ```
 - 인덕터와 커패시터 해석은 다음과 같다.
   - iC= C*(dv/dt)
     - 전압 변화량은 이전 프레임 전압과 현제 프레임 전압의 차이로 한다.
     - dt는 설정된 프레임 당 시간이다.(기본값 0.01초)
   - iL= integral(v)*(dt/L)
     - integral(v)는 전압의 누적량이다.
     ```C#
     CM.loops[i].components[j].GetComponent<Resistor>().intergral += term * CM.loops[i].components[j].GetComponent<Resistor>().voltage;// 각 프레임마다 
     ```
   - 인덕터와 커패시터는 각각 직류 전원인가 시 단락, 개방 처리 된다.(ComponentManager.cleanUpLink())
     - 단락 처리 시 인덕터의 양 연결점에 있는 소자들을 서로 연결한다.
     - 개방처리 시 커패시터의 양 연결점에 있는 소자들의 해당 연결점 정보를 지운다.
     - 위 처리들은 스위치소자의 on/off에도 동일하게 적용된다.
 - 회로 해석 중 접지가 없다면 노드 중 가장 연결된 소자들이 많은 노드가 접지처리 된다.(CircuitAnalyzer.setGround())
   - 접지처리된 노드는 값이 0이므로 방정식에서 제거된다.(CircuitAnalyzer.applyGround())
 - 교류전원은 주파수와 위상을 설정할 수 있다.
   - 주파수 변화 시 오실로스코프 분석을 위해 시간이 스케일링된다.(CircuitAnalyzer.setTerm())
   ```C#
    //일부 발췌
    int a = (int)(term / x) / 10;// 주파수가 10의 단위로 변할 시 시간변화량이 줄어듦 
    int b = 1;
    while (a >= 10)
    {
       a = (int)(a / 10);
       b += 1;
    }
    term = term / Mathf.Pow(10, b);// ex) f=15 -> term/=10  f=132 -> term/=100
   ```
   - 시간 스케일링 시 시간 텍스트가 반짝인다.
 - 회로 상태를 보여주기 위해 오실로스코프 기능을 지원한다.
   - 전압 또는 전류를 토글 기능으로 확인 가능
   - 스코프의 시간축 변화량 변경 가능
   ```C#
            interval = int.Parse(intervalText.text);// 텍스트필드 입력값을 받음
            line[i].positionCount = interval + 1;// 선의 갯수 설정
            graphYPos[i] = new List<float>();
            line[i].SetPosition(0, new Vector2(-50, 0));// 오실로스코프의 길이는 50
            graphYPos[i].Add(0);
            for (int j = 1; j < line[i].positionCount - 1; j++)// 선의 갯수(인터벌 값)에 따라 graphYPos리스트에 값을 추가
            {
                line[i].SetPosition(j, new Vector2(-50 + ((50 / (float)interval) * j), 0));
                graphYPos[i].Add(0);
            }
            line[i].SetPosition(line[i].positionCount - 1, new Vector2(0, 0));
            graphYPos[i].Add(0);
   
   ```
   - 스코프의 범위는 그래프의 절대값 최대치의 1.3배까지이다. 
 
