document.querySelectorAll('.tab-button').forEach(button => {
  button.addEventListener('click', () => {
    const tab = button.dataset.tab;

    // 모든 버튼에서 active 제거
    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
    button.classList.add('active');

    // 모든 탭 콘텐츠 숨기기
    document.querySelectorAll('.tab-content').forEach(content => {
      content.classList.remove('active');
    });

    // 선택된 탭만 표시
    const activeContent = document.getElementById(tab);
    if (activeContent) {
      activeContent.classList.add('active');
    }
  });
});

// 초기 탭 로드
window.addEventListener('DOMContentLoaded', () => {
  const activeTab = document.querySelector('.tab-button.active')?.dataset.tab;
  if (activeTab) {
    const tabElement = document.getElementById(activeTab);
    if (tabElement) {
      tabElement.classList.add('active');
    }
  }
});
