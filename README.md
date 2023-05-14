# AR_Docent

#Overview: What is AR_Docent?
<p align= "center">
<img width= "400" src= "https://user-images.githubusercontent.com/69339846/178733741-3abc68e7-9e2e-4d40-ae76-de35914f1f71.jpeg">
</p>
AR_Docent는 KMU graduation exhibition에서 사용할 AR 프로젝트입니다.
IPad에서 구동되며 Unity AR Foundation을 사용합니다.

# User's Manual

# Target Environment

Unity2021.3.2.f1 Apple Silicon.

# Target Platforms

iOS, liDAR센서가 내장된 기기.

# Todo List
|task|check|설명|
|-|-|-|
|image tracking|<ul><li>- [x] </li></ul>|작품을 트래킹하는데 성공.|
|docent moving|<ul><li>- [x] </li></ul>|nev mesh를 활용한 docent의 이동|
|작품 설명 display|<ul><li>- [ ] </li></ul>|작품 설명을 text로 벽면에 display.|
|tracking image upload server|<ul><li>- [x] </li></ul>|트래킹할 작품이미지와 설명을 업로드하는 서버|
|docent animation|<ul><li>- [x] </li></ul>|docent의 안내 rig animation|
|docent modeling|<ul><li>- [ ] </li></ul>| docent디자인, 모델링|

# 구현 상태
https://youtu.be/PZrLlL9cG5M
<img width="960" alt="스크린샷 2023-05-14 오후 2 33 01" src="https://github.com/AR-Docent/AR_Docent/assets/69339846/17821f23-d5aa-429d-b3c8-8cbcd7a06793">

(uml 쓰는 법을 공부해야함)
![ARDocent](https://github.com/AR-Docent/AR_Docent/assets/69339846/72f42cd5-b668-42ce-9075-f8276999a946)


# 향후 계획
Unity Native call을 사용하거나 Apple ML API를 사용하거나 Asset Store을 사용하여 Voice to Text 기능 구현.
https://developer.apple.com/kr/machine-learning/api/#speech

AutoGPT를 사용하여 질문에 대한 답변 생성
https://github.com/Significant-Gravitas/Auto-GPT

database 설계 변경

ardocent 사이트에 로그인 추가.

