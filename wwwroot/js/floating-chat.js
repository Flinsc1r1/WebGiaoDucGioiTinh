(function () {
    'use strict';

    function appendBubble(container, text, isUser) {
        var wrap = document.createElement('div');
        wrap.className = isUser ? 'flex justify-end' : 'flex justify-start';
        var bubble = document.createElement('div');
        bubble.className = isUser
            ? 'max-w-[85%] rounded-2xl rounded-br-md bg-gradient-to-br from-sky-400 to-teal-500 px-3.5 py-2.5 text-sm leading-relaxed text-white shadow-md'
            : 'max-w-[90%] rounded-2xl rounded-bl-md border border-emerald-200/80 bg-white/95 px-3.5 py-2.5 text-sm leading-relaxed text-slate-800 shadow-sm';

        // Hỗ trợ hiển thị xuống dòng từ AI
        bubble.innerHTML = text.replace(/\n/g, '<br/>');
        wrap.appendChild(bubble);
        container.appendChild(wrap);
        container.scrollTop = container.scrollHeight;
    }

    function init() {
        var fab = document.getElementById('floating-chat-fab');
        var panel = document.getElementById('floating-chat-panel');
        var messagesEl = document.getElementById('floating-chat-messages');
        var form = document.getElementById('floating-chat-form');
        var input = document.getElementById('floating-chat-input');

        if (!fab || !panel || !messagesEl || !form || !input) return;

        // Mở/Đóng chat
        fab.addEventListener('click', function () {
            panel.classList.toggle('hidden');
            if (!panel.classList.contains('hidden')) {
                input.focus();
                if (messagesEl.children.length === 0) {
                    appendBubble(messagesEl, 'Chào bạn! Mình là Trợ lý nhỏ. Bạn cần mình tư vấn điều gì về sức khỏe và tâm lý không? 😊', false);
                }
            }
        });

        // Xử lý gửi tin nhắn
        form.addEventListener('submit', async function (e) {
            e.preventDefault();
            var text = input.value.trim();
            if (!text) return;

            appendBubble(messagesEl, text, true);
            input.value = '';

            // HIỆN HIỆU ỨNG ĐANG TRẢ LỜI
            var loadingBubble = document.createElement('div');
            loadingBubble.innerText = "Trợ lý nhỏ đang suy nghĩ...";
            messagesEl.appendChild(loadingBubble);

            try {
                const response = await fetch('/api/chat/ask', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ prompt: text })
                });

                const data = await response.json();
                messagesEl.removeChild(loadingBubble); // Xóa dòng đang suy nghĩ

                // Giải mã kết quả từ Gemini
                if (data.candidates && data.candidates[0].content.parts[0].text) {
                    var aiReply = data.candidates[0].content.parts[0].text;
                    appendBubble(messagesEl, aiReply, false);
                } else {
                    appendBubble(messagesEl, "Xin lỗi, mình gặp chút trục trặc. Bạn thử lại nhé!", false);
                }

            } catch (error) {
                messagesEl.removeChild(loadingBubble);
                appendBubble(messagesEl, "Lỗi kết nối rồi Thiên ơi. Kiểm tra lại API Key nhé!", false);
                console.error(error);
            }
        });
    }

    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init);
    else init();
})();