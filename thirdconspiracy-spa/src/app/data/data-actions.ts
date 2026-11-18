import { parseDocument, visit } from 'yaml';

export type DataActionId =
  | 'pretty-json'
  | 'pretty-xml'
  | 'json-to-yaml'
  | 'decode-jwt'
  | 'encode-base64'
  | 'decode-base64'
  | 'encode-url'
  | 'decode-url'
  | 'encode-html'
  | 'decode-html';

export interface DataAction {
  id: DataActionId;
  label: string;
  inputLabel: string;
  outputLabel: string;
  example: string;
  transform: (input: string) => string;
}

/* Modern APIs only without fallback */

type JsonWithRaw = JSON & { rawJSON: (text: string) => unknown };
type Reviver = (
  this: unknown,
  key: string,
  value: unknown,
  context?: { source?: string },
) => unknown;

function parseJsonPreservingNumbers(text: string): unknown {
  const { rawJSON } = JSON as JsonWithRaw;
  const reviver: Reviver = (_key, value, context) =>
    typeof value === 'number' && context?.source !== undefined ? rawJSON(context.source) : value;
  return JSON.parse(text, reviver);
}

export const prettyPrintJson = (input: string): string => {
  if (input.trim() === '') return '';
  try {
    return JSON.stringify(parseJsonPreservingNumbers(input), null, 2);
  } catch (e) {
    throw new Error(`Invalid JSON: ${(e as Error).message}`);
  }
};

export const prettyPrintXml = (input: string): string => {
  if (input.trim() === '') return '';
  const doc = new DOMParser().parseFromString(input, 'application/xml');
  const error = findXmlParseError(doc);
  if (error) throw new Error(`Invalid XML: ${error}`);

  // The XML declaration isn't a DOM node, so carry it over from the source.
  const declaration = /^﻿?(<\?xml\s[\s\S]*?\?>)/.exec(input)?.[1];
  const body = [...doc.childNodes].filter((n) => !isBlankText(n)).map((n) => prettyXmlNode(n, 0));
  return (declaration ? [declaration, ...body] : body).join('\n');
};

export const jsonToYaml = (input: string): string => {
  if (input.trim() === '') return '';
  const doc = parseDocument(input, { intAsBigInt: true });
  if (doc.errors.length)
    throw new Error(`Invalid JSON/YAML: ${doc.errors[0].message.split('\n')[0]}`);

  // Drop JSON's quoting so strings are written plain (YAML still quotes when it must).
  visit(doc, {
    Scalar(_key, node) {
      if (typeof node.value === 'string') node.type = undefined;
    },
  });
  return doc
    .toString({ indent: 2, collectionStyle: 'block', lineWidth: 0, blockQuote: 'literal' })
    .replace(/\n$/, '');
};

/** Decodes a JWT's header and payload (the signature is not verified). */
export const decodeJwt = (input: string): string => {
  const token = input.trim().replace(/^Bearer\s+/i, '');
  if (token === '') return '';
  const parts = token.split('.');
  if (parts.length === 5) throw new Error('Invalid JWT: this is an encrypted JWE, which can\'t be decoded without the key');
  if (parts.length !== 3) throw new Error(`Invalid JWT: expected 3 dot-separated parts, found ${parts.length}`);

  const [headerPart, payloadPart, signature] = parts;
  const header = tryDecodeJwtSegment(headerPart);
  if (!header) throw new Error('Invalid JWT: header is not Base64URL-encoded JSON');
  const payload = tryDecodeJwtSegment(payloadPart);
  if (!payload) throw new Error('Invalid JWT: payload is not Base64URL-encoded JSON');

  const decoded: Record<string, unknown> = {
    header: parseJsonPreservingNumbers(header.text),
    payload: parseJsonPreservingNumbers(payload.text),
    signature,
  };
  if (header.alphabet === 'swapped' || payload.alphabet === 'swapped') {
    decoded['warning'] = 'Non-standard encoding: the "-" and "_" characters are swapped';
  }
  return JSON.stringify(decoded, null, 2);
};

export const encodeBase64 = (input: string): string => new TextEncoder().encode(input).toBase64();

/** Decodes standard or URL-safe Base64 (padding optional) to UTF-8 text. */
export const decodeBase64 = (input: string): string => {
  const text = input.trim();
  if (text === '') return '';
  let bytes: Uint8Array;
  try {
    bytes = Uint8Array.fromBase64(text, { alphabet: /[-_]/.test(text) ? 'base64url' : 'base64' });
  } catch {
    throw new Error('Invalid Base64: unexpected character or length');
  }
  try {
    return utf8.decode(bytes);
  } catch {
    throw new Error('Invalid Base64: decoded bytes are not UTF-8 text');
  }
};

export const encodeUrl = (input: string): string => encodeURIComponent(input);

export const decodeUrl = (input: string): string => {
  try {
    return decodeURIComponent(input);
  } catch {
    throw new Error('Invalid URL encoding: malformed % sequence');
  }
};

export const encodeHtml = (input: string): string => input.replace(/[&<>"']/g, (c) => HTML_ENTITIES[c]);

export const decodeHtml = (input: string): string =>
  new DOMParser().parseFromString(input, 'text/html').documentElement.textContent ?? '';

const sampleJson = '{"name":"ThirdConspiracy","team":"engineering","roles":["admin","dev"],"active":true}';
const sampleXml =
  '<?xml version="1.0" encoding="UTF-8"?><team name="ThirdConspiracy"><!-- Members --><member role="admin">Ada</member><member role="dev">Grace <em>Hopper</em></member><empty/></team>';
// Signed with a throwaway secret; for demo only.
const sampleJwt =
  'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlRoaXJkQ29uc3BpcmFjeSIsInJvbGVzIjpbImFkbWluIiwiZGV2Il0sImlhdCI6MTc2Mzg1NjAwMCwiZXhwIjoxNzYzOTQyNDAwfQ.7TXJaKL5csRsf6gzNDOJo9oBhX7rl3kIACxmiBo057w';
const sampleText = 'Hello, ThirdConspiracy! ✓';
const sampleUrl = 'https://example.com/search?q=third conspiracy&lang=en-US';
const sampleHtml = '<p class="note">Tom & Jerry\'s "Cartoon" <3</p>';

const XML_INDENT = '  ';
/** Namespaces of the <parsererror> element browsers insert for bad XML (Chromium/WebKit, Firefox). */
const PARSER_ERROR_NAMESPACES = [
  'http://www.w3.org/1999/xhtml',
  'http://www.mozilla.org/newlayout/xml/parsererror.xml',
];

/** Returns the browser's parse error message, or null when the XML is well-formed. */
function findXmlParseError(doc: Document): string | null {
  for (const ns of PARSER_ERROR_NAMESPACES) {
    const error = doc.getElementsByTagNameNS(ns, 'parsererror')[0];
    if (!error) continue;
    // Chromium/WebKit put the message in a <div>; Firefox puts it in the first text node,
    // followed by a <sourcetext> excerpt.
    const message = error.querySelector('div')?.textContent ?? error.firstChild?.textContent ?? '';
    return message.trim().split('\n')[0] || 'could not parse document';
  }
  return null;
}

const escapeXmlText = (text: string): string =>
  text.replace(/[&<>]/g, (c) => (c === '&' ? '&amp;' : c === '<' ? '&lt;' : '&gt;'));

const escapeXmlAttr = (value: string): string =>
  value.replace(/[&<>"\t\n\r]/g, (c) => XML_ATTR_ENTITIES[c]);

const XML_ATTR_ENTITIES: Readonly<Record<string, string>> = {
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;',
  '"': '&quot;',
  '\t': '&#9;',
  '\n': '&#10;',
  '\r': '&#13;',
};

const isBlankText = (node: Node): boolean =>
  node.nodeType === Node.TEXT_NODE && (node.textContent ?? '').trim() === '';

function xmlOpenTag(el: Element, selfClosing = false): string {
  const attrs = [...el.attributes].map((a) => ` ${a.name}="${escapeXmlAttr(a.value)}"`).join('');
  return `<${el.tagName}${attrs}${selfClosing ? '/' : ''}>`;
}

/** Serializes a node on one line, keeping its text exactly as written. */
function inlineXmlNode(node: Node): string {
  switch (node.nodeType) {
    case Node.ELEMENT_NODE: {
      const el = node as Element;
      if (!el.childNodes.length) return xmlOpenTag(el, true);
      return `${xmlOpenTag(el)}${[...el.childNodes].map(inlineXmlNode).join('')}</${el.tagName}>`;
    }
    case Node.TEXT_NODE:
      return escapeXmlText(node.textContent ?? '');
    case Node.CDATA_SECTION_NODE:
      return `<![CDATA[${node.textContent ?? ''}]]>`;
    case Node.COMMENT_NODE:
      return `<!--${node.textContent ?? ''}-->`;
    case Node.PROCESSING_INSTRUCTION_NODE: {
      const pi = node as ProcessingInstruction;
      return `<?${pi.target}${pi.data ? ` ${pi.data}` : ''}?>`;
    }
    case Node.DOCUMENT_TYPE_NODE:
      return new XMLSerializer().serializeToString(node);
    default:
      return '';
  }
}

/**
 * Indents elements that only contain other elements, comments or PIs. Elements holding
 * text (including mixed content) stay on one line so whitespace in the text isn't changed.
 */
function prettyXmlNode(node: Node, depth: number): string {
  const indent = XML_INDENT.repeat(depth);
  if (node.nodeType !== Node.ELEMENT_NODE) {
    const text = node.nodeType === Node.TEXT_NODE ? escapeXmlText((node.textContent ?? '').trim()) : inlineXmlNode(node);
    return indent + text;
  }

  const el = node as Element;
  const children = [...el.childNodes].filter((n) => !isBlankText(n));
  const hasText = children.some((n) => n.nodeType === Node.TEXT_NODE || n.nodeType === Node.CDATA_SECTION_NODE);
  if (!children.length) return indent + xmlOpenTag(el, true);
  if (hasText) return indent + inlineXmlNode(el);

  return [
    indent + xmlOpenTag(el),
    ...children.map((child) => prettyXmlNode(child, depth + 1)),
    `${indent}</${el.tagName}>`,
  ].join('\n');
}

const utf8 = new TextDecoder('utf-8', { fatal: true });

const HTML_ENTITIES: Readonly<Record<string, string>> = {
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;',
  '"': '&quot;',
  "'": '&#39;',
};

/** 'swapped': some encoders mix up "-" and "_", which still decodes but to garbage. */
type JwtAlphabet = 'base64url' | 'swapped';
const JWT_ALPHABETS: readonly JwtAlphabet[] = ['base64url', 'swapped'];

/** Decodes one JWT segment to JSON text, trying each alphabet until the result is a JSON object. */
function tryDecodeJwtSegment(segment: string): { text: string; alphabet: JwtAlphabet } | null {
  // Also accept standard Base64 ("+", "/", "=" padding) by normalizing it to Base64URL.
  const normalized = segment.replace(/=+$/, '').replace(/[+/]/g, (c) => (c === '+' ? '-' : '_'));
  for (const alphabet of JWT_ALPHABETS) {
    const text =
      alphabet === 'swapped' ? normalized.replace(/[-_]/g, (c) => (c === '-' ? '_' : '-')) : normalized;
    try {
      const decoded = utf8.decode(Uint8Array.fromBase64(text, { alphabet: 'base64url' }));
      if (isJson(decoded)) return { text: decoded, alphabet };
    } catch {
      // Not valid in this alphabet; try the next.
    }
  }
  return null;
}

/** True when the text is a JSON object or array (not a bare string, number or literal). */
export function isJson(text: string): boolean {
  try {
    const value: unknown = JSON.parse(text);
    return typeof value === 'object' && value !== null;
  } catch {
    return false;
  }
}

/** True when the text is a well-formed XML document. */
export const isXml = (text: string): boolean =>
  text.trimStart().startsWith('<') &&
  findXmlParseError(new DOMParser().parseFromString(text, 'application/xml')) === null;

export const DATA_ACTIONS: readonly DataAction[] = [
  {
    id: 'pretty-json',
    label: 'Pretty Print: JSON',
    inputLabel: 'JSON',
    outputLabel: 'Formatted JSON',
    example: sampleJson,
    transform: prettyPrintJson,
  },
  {
    id: 'pretty-xml',
    label: 'Pretty Print: XML',
    inputLabel: 'XML',
    outputLabel: 'Formatted XML',
    example: sampleXml,
    transform: prettyPrintXml,
  },
  {
    id: 'json-to-yaml',
    label: 'Convert: JSON to YAML',
    inputLabel: 'JSON',
    outputLabel: 'YAML',
    example: sampleJson,
    transform: jsonToYaml,
  },
  {
    id: 'decode-jwt',
    label: 'Decode: JWT',
    inputLabel: 'JWT',
    outputLabel: 'Header and Payload',
    example: sampleJwt,
    transform: decodeJwt,
  },
  {
    id: 'encode-base64',
    label: 'Encode: Base64',
    inputLabel: 'Text',
    outputLabel: 'Base64',
    example: sampleText,
    transform: encodeBase64,
  },
  {
    id: 'decode-base64',
    label: 'Decode: Base64',
    inputLabel: 'Base64',
    outputLabel: 'Text',
    example: encodeBase64(sampleText),
    transform: decodeBase64,
  },
  {
    id: 'encode-url',
    label: 'Encode: URL',
    inputLabel: 'Text',
    outputLabel: 'URL-Encoded',
    example: sampleUrl,
    transform: encodeUrl,
  },
  {
    id: 'decode-url',
    label: 'Decode: URL',
    inputLabel: 'URL-Encoded',
    outputLabel: 'Text',
    example: encodeUrl(sampleUrl),
    transform: decodeUrl,
  },
  {
    id: 'encode-html',
    label: 'Encode: HTML',
    inputLabel: 'Text',
    outputLabel: 'HTML-Encoded',
    example: sampleHtml,
    transform: encodeHtml,
  },
  {
    id: 'decode-html',
    label: 'Decode: HTML',
    inputLabel: 'HTML-Encoded',
    outputLabel: 'Text',
    example: encodeHtml(sampleHtml),
    transform: decodeHtml,
  },
];
