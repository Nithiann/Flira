import { Pipe, PipeTransform, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'markdown',
  standalone: true
})
export class MarkdownPipe implements PipeTransform {
  private readonly sanitizer = inject(DomSanitizer);

  transform(value: string | null | undefined): SafeHtml {
    if (!value) return '';

    // Escape basic HTML tags to prevent XSS before rendering markdown
    let escaped = value
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');

    // 1. Code blocks: ```code```
    escaped = escaped.replace(/```([\s\S]*?)```/g, '<pre><code>$1</code></pre>');

    // 2. Inline code: `code`
    escaped = escaped.replace(/`([^`]+)`/g, '<code>$1</code>');

    // 3. Headers: #, ##, ###
    escaped = escaped.replace(/^### (.*?)$/gm, '<h3>$1</h3>');
    escaped = escaped.replace(/^## (.*?)$/gm, '<h2>$1</h2>');
    escaped = escaped.replace(/^# (.*?)$/gm, '<h1>$1</h1>');

    // 4. Bold: **text**
    escaped = escaped.replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>');

    // 5. Italic: *text* or _text_
    escaped = escaped.replace(/\*([^*]+)\*/g, '<em>$1</em>');
    escaped = escaped.replace(/_([^_]+)_/g, '<em>$1</em>');

    // 6. Links: [text](url)
    escaped = escaped.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2" target="_blank" class="markdown-link">$1</a>');

    // 7. Unordered Lists: - item or * item
    escaped = escaped.replace(/^\s*[-*]\s+(.*?)$/gm, '<li>$1</li>');
    // Wrap consecutive list items in <ul>
    escaped = escaped.replace(/(<li>.*<\/li>)/g, '<ul>$1</ul>');
    // Clean up double ul tags
    escaped = escaped.replace(/<\/ul>\s*<ul>/g, '');

    // 8. Mentions: @username or @UserId (starts with letter, number or dot/hyphen)
    escaped = escaped.replace(/@([a-zA-Z0-9.\-_]+)/g, '<span class="mention-badge">@$1</span>');

    // 9. Line breaks
    escaped = escaped.replace(/\n/g, '<br/>');

    return this.sanitizer.bypassSecurityTrustHtml(escaped);
  }
}
